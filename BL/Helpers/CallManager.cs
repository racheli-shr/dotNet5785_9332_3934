using BL.Helpers;
using BLImplementation;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

//namespace BL.Helpers
namespace Helpers;

internal static class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static readonly IDal c_dal = Factory.Get; //stage 4



    #region check address
    /// <summary>
    /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
    /// </summary>
    /// <param name="address">The address to get the coordinates for.</param>
    /// <returns>A tuple containing the latitude and longitude of the address.</returns>
    /// <exception cref="BO.BlValidationException">Thrown when the address is invalid.</exception>
    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BO.Exceptions.BLInvalidDataException("The address is invalid.");
        }

        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(url);

                var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);

                if (result == null || result.Length == 0)
                {
                    throw new BO.Exceptions.BLInvalidDataException("The address is invalid.");
                }

                double latitude = double.Parse(result[0].Latitude);
                double longitude = double.Parse(result[0].Longitude);

                return (latitude, longitude);
            }
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.BLInvalidDataException("Error retrieving coordinates" + ex.Message);
        }
    }


    /// <summary>
    /// Represents the structure of a geocoding response.
    /// </summary>
    private class GeocodeResponse
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; } // Latitude as string

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } // Longitude as string

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } // Full address representation
    }
    #endregion

    internal static DO.Call ConvertBOToDO(BO.Call call) =>
    new DO.Call
    {
        Id = call.Id,
        CallType = (DO.Enums.CallType)call.CallType,
        FullAdress = call.FullAddress!,
        Latitude = call.Latitude,
        longtitude = call.longtitude,
        OpeningCallTime = call.OpeningTime,
        Description = call.Description,
        MaxTimeToEnd = call.MaxFinishTime
    };


    /// <summary>
    /// Converts a DO.StudentCall object to a BO.CallInList object.
    /// </summary>
    /// <param name="studentCall">The DO.StudentCall object to convert.</param>
    /// <returns>A BO.CallInList object representing the student call.</returns>
    internal static BO.Enums.CallStatus GetCallStatus(DateTime? MaxTimeToEnd,int callId)
    {
        if (MaxTimeToEnd == null)
            return  BO.Enums.CallStatus.Open;
        var lastAssignment = c_dal.Assignment.ReadAll(a => a.CallId == callId)
                                         .OrderBy(a => a.EntryTimeForTreatment)
                                         .LastOrDefault();

        bool isCallExpired = MaxTimeToEnd!=null &&  MaxTimeToEnd < AdminManager.Now;
        bool isCallInRisk = MaxTimeToEnd!=null && MaxTimeToEnd - AdminManager.Now < AdminManager.MaxRange;

        if (isCallExpired)
            return BO.Enums.CallStatus.Expired;

        if (lastAssignment == null)
            return isCallInRisk ? BO.Enums.CallStatus.OpenAtRisk : BO.Enums.CallStatus.Open;

        if (isCallInRisk)
            return BO.Enums.CallStatus.InProgressAtRisk;

        return lastAssignment.AssignmentStatus switch
        {
            DO.Enums.AssignmentStatus.TREATED => BO.Enums.CallStatus.Closed,
            DO.Enums.AssignmentStatus.SELF_CANCELLED => BO.Enums.CallStatus.Open,
            DO.Enums.AssignmentStatus.MANAGER_CANCELLED => BO.Enums.CallStatus.Open,
            DO.Enums.AssignmentStatus.EXPIRED => BO.Enums.CallStatus.Expired,
            DO.Enums.AssignmentStatus.NONE => BO.Enums.CallStatus.InProgress,
            DO.Enums.AssignmentStatus.AssignedAndInProgress => BO.Enums.CallStatus.InProgress

        };
    }



    
    static internal void Validation(BO.Call call)
    {
        if (call.OpeningTime > call.MaxFinishTime)
        {
            throw new BO.Exceptions.BLInvalidDataException($"invalid data entry time cannot be initialize after maxtime");
        }
    }

    /// <summary>
    /// Calculates the status of a student call based on its properties and the current time.
    /// </summary>
    /// <param name="studentCall">The student call to calculate the status for.</param>
    /// <returns>The calculated status of the student call.</returns>
    internal static BO.Enums.CallStatus CalculateCallStatus(DO.Call call)
    {
        DO.Assignment lastAssignment;
        lock (AdminManager.BlMutex) //stage 7
            lastAssignment = c_dal.Assignment
    .ReadAll(a => a.CallId == call.Id)
    .OrderBy(a => a.EntryTimeForTreatment)
    .LastOrDefault();
        bool isCallExpired = call.MaxTimeToEnd.HasValue && call.MaxTimeToEnd < AdminManager.Now;
        bool isCallInRisk = call.MaxTimeToEnd.HasValue && call.MaxTimeToEnd - AdminManager.Now < AdminManager.MaxRange;
        if (isCallExpired)
        {
            return BO.Enums.CallStatus.Expired;
        }

        if (lastAssignment == null)
            return isCallInRisk ? BO.Enums.CallStatus.OpenAtRisk : BO.Enums.CallStatus.Open;

        if (isCallInRisk)
            return BO.Enums.CallStatus.InProgressAtRisk;

        return lastAssignment?.AssignmentStatus switch
        {
            DO.Enums.AssignmentStatus.TREATED => BO.Enums.CallStatus.Closed,
            DO.Enums.AssignmentStatus.SELF_CANCELLED => BO.Enums.CallStatus.Open,
            DO.Enums.AssignmentStatus.MANAGER_CANCELLED => BO.Enums.CallStatus.Open,
            DO.Enums.AssignmentStatus.AssignedAndInProgress => BO.Enums.CallStatus.InProgress,
            DO.Enums.AssignmentStatus.EXPIRED => BO.Enums.CallStatus.Expired,
            _ or null => BO.Enums.CallStatus.Open
        };
    }


    /// <summary>
    /// מבצע בדיקות תקינות לפורמט של קריאה.
    /// בודק שדות חובה, ערכים מספריים תקינים, וטווחי נתונים.
    /// </summary>
    /// <param name="boCall">אובייקט הקריאה לבדיקה</param>
    /// <exception cref="Exception">נזרק כאשר שדה חובה חסר</exception>
    public static void ValidateCall(BO.Call boCall)
    {
        // בדיקת תיאור הקריאה
        if (string.IsNullOrWhiteSpace(boCall.Description))
            throw new Exception("Description must not be null or empty.");

        if (boCall.Description.Length > 255)
            throw new Exception("Description must not exceed 255 characters.");

        // בדיקת כתובת
        if (string.IsNullOrWhiteSpace(boCall.FullAddress))
            throw new Exception("Address must not be null or empty.");

        if (boCall.FullAddress.Length > 255)
            throw new Exception("Address must not exceed 255 characters.");

        // בדיקת סוג קריאה
        if (!Enum.IsDefined(typeof(BO.Enums.CallType), boCall.CallType))
            throw new Exception($"Invalid TypeCall value: {boCall.CallType}");
    }



    /// <summary>
    /// מבצע בדיקות היגיון לקריאה, למשל וידוא שתאריך הסיום גדול מתאריך הפתיחה.
    /// </summary>
    /// <param name="boCall">אובייקט הקריאה לבדיקה</param>
    /// <exception cref="ArgumentException">
    /// נזרק כאשר תנאי ההיגיון אינם מתקיימים (לדוגמה, תאריך הסיום קטן מתאריך הפתיחה).
    /// </exception>
    public static void ValidateLogicalCall(BO.Call boCall)
    {
        if (boCall.MaxFinishTime.HasValue && boCall.MaxFinishTime.Value <= boCall.OpeningTime)
        {
            throw new ArgumentException("MaxTimeFinishCall must be later than OpeningTimeCall.", nameof(boCall.MaxFinishTime));
        }
    }

    /// <summary>
    /// Updates the status of calls that have passed their final time.
    /// </summary>
    internal static void PeriodicCallStatusUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex) //stage 7
            calls = c_dal.Call.ReadAll(c => c.MaxTimeToEnd.HasValue && c.MaxTimeToEnd <= newClock);

        foreach (var call in calls)
        {
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex) //stage 7
                assignments = c_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();

            if (assignments == null || !assignments.Any())
            {
                var newAssignment = new DO.Assignment
                {
                    CallId = call.Id,
                    VolunteerId = 0,
                    EntryTimeForTreatment = newClock,
                    AssignmentStatus = DO.Enums.AssignmentStatus.EXPIRED,
                    ActualTreatmentEndTime = newClock
                };
                lock (AdminManager.BlMutex) //stage 7
                    c_dal.Assignment.Create(newAssignment);
                Observers.NotifyItemUpdated(newAssignment.CallId);
                Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyListUpdated();

            }
            else
            {
                foreach (var assignment in assignments.Where(a => a.ActualTreatmentEndTime == null))
                {
                    var updated = assignment with
                    {
                        ActualTreatmentEndTime = newClock,
                        AssignmentStatus = DO.Enums.AssignmentStatus.EXPIRED
                    };
                    lock (AdminManager.BlMutex) //stage 7
                        c_dal.Assignment.Update(updated);
                    Observers.NotifyItemUpdated(updated.CallId);
                    VolunteerManager.Observers.NotifyItemUpdated(updated.VolunteerId);
                    VolunteerManager.Observers.NotifyListUpdated();
                    Observers.NotifyListUpdated();
                }
            }
        }

        Observers.NotifyListUpdated();
    }
}
