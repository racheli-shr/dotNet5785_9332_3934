
using BL.Helpers;
using BLImplementation;
using DalApi;
using System.ComponentModel;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal v_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5
                                                       // Define the delegate for converting BO.Volunteer to DO.Volunteer


    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;


    internal static void TutorSimulator()
    {
        BLApi.IVolunteer volunteerImplementation= new VolunteerImplementation();
        BLApi.ICall callImplementation = new CallImplementation();
        
        Random rnd = new Random();
        int toCancel = rnd.Next(1, 6);
        int toAssign = rnd.Next(1, 6);
        var activeDOTutors = v_dal.Volunteer.ReadAll(tutor => tutor.IsActive);
        IEnumerable<BO.Volunteer> activeTutors = activeDOTutors.Select(t => volunteerImplementation.Read(t.Id));
        foreach (var tutor in activeTutors)
        {
            BO.CallInProgress currentCallInProgress = tutor.CurrentCallInProgress;
            if (currentCallInProgress == null)
            {
                toAssign = rnd.Next(1, 11);
                if (toAssign == 1)
                {
                    var mostCommonSubject = callImplementation.GetClosedCallsByVolunteer(tutor.Id)
                     .GroupBy(c => c.CallType)
                     .OrderByDescending(g => g.Count())
                     .Select(g => (BO.Enums.CallType?)g.Key)
                     .FirstOrDefault();

                    IEnumerable<BO.OpenCallInList> openCalls;
                    if (mostCommonSubject != null)
                        openCalls = callImplementation.FilterOpenCalls(tutor.Id, BO.Enums.OpenCallInListFields.CallType, mostCommonSubject);
                    else
                        openCalls = callImplementation.FilterOpenCalls(tutor.Id);

                    openCalls.OrderBy(c => c.DistanceFromVolunteer);
                    if (openCalls.Any())
                        callImplementation.AssignCallToVolunteer(tutor.Id, openCalls.FirstOrDefault().Id);

                }
            }
            else
            {
                DateTime entryTime = currentCallInProgress.EntryTimeToHandle;
                int addTime = 7 + (int)Math.Floor((double)currentCallInProgress.DistanceFromVolunteer) / 2;
                if (entryTime.AddDays(addTime) <= AdminManager.Now)
                {
                    callImplementation.CompleteCallTreatment(tutor.Id, currentCallInProgress.Id);
                }
                else
                {
                    toCancel = rnd.Next(1, 11);
                    if (toCancel == 1)
                        callImplementation.DeleteAssignmentToCall(currentCallInProgress.Id);
                }


            }

        }
    }
    internal static bool IsValidName(string name)
    {
        // בדיקה שהאורך בין 2 ל-50 תווים
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 50)
            return false;

        // ביטוי רגולרי: שם יכול להכיל רק אותיות (עברית/אנגלית) ורווחים, אך לא להתחיל או להסתיים ברווח
        string pattern = @"^[A-Za-zא-ת]+(?: [A-Za-zא-ת]+)*$";

        return Regex.IsMatch(name, pattern);
    }
    
    public static DO.Volunteer ConvertBOToDO(BO.Volunteer input) =>
        new DO.Volunteer
        {
            Id = input.Id,                    // Mapping Id
            FullName = input.FullName,        // Mapping fullName (ensure casing matches)
            Phone = input.Phone,              // Mapping phone
            Email = input.Email,              // Mapping email
            FullAdress = input.FullAddress,          // Mapping address                            
            // Password = input.Password,     // Uncomment if needed
            IsActive = input.IsActive,  // Mapping isAvailable
            Role = (DO.Enums.Role)input.Role,                // Mapping Role
            DistanceType = (DO.Enums.DistanceType)input.DistanceType, // Mapping typeDistance
            MaxDistance = input.MaxDistance,  // Mapping maxDistance
            Latitude = input.Latitude,        // Mapping Latitude
            longtitude = input.longtitude       // Mapping longtitude
        };
    internal static bool IsValidIsraeliID(string id)
    {
        if (id.Length != 9 || !long.TryParse(id, out _))
            return false;
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = id[i] - '0'; //מחזיר לי את הערך המספרי של התו
            if (i % 2 == 0)
                sum += digit;
            else
            {
                int doubled = digit * 2;
                sum += (doubled > 9) ? doubled - 9 : doubled;
            }
        }
        int checksum = (10 - (sum % 10)) % 10;
        return checksum == (id[8] - '0');
    }

    internal static double? GetDistance(double? volLatitude, double? vollongtitude, double? callLatitude, double? calllongtitude, DO.Enums.DistanceType? typeDistance)
    {

        switch (typeDistance)
        {
            case DO.Enums.DistanceType.airDistance:
                return CalculateHaversineDistance(volLatitude, vollongtitude, callLatitude, calllongtitude);

            case DO.Enums.DistanceType.walkDistance:
                return SimulatedWalkingDistance(volLatitude, vollongtitude, callLatitude, calllongtitude);

            case DO.Enums.DistanceType.driveDistance:
                return SimulatedDrivingDistance(volLatitude, vollongtitude, callLatitude, calllongtitude);

            default:
                return 0.0;
        }

    }

    // חישוב מרחק אווירי נוסחת Haversine
    private static double? CalculateHaversineDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        const double R = 6371; // רדיוס כדור הארץ בק"מ
        double? dLat = DegreesToRadians(lat2 - lat1);
        double? dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
                   Math.Cos((double)DegreesToRadians(lat1)) * Math.Cos((double)DegreesToRadians(lat2)) *
                   Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // המרחק בקילומטרים
    }

    // המרחק להליכה (סימולציה בסיסית)
    private static double? SimulatedWalkingDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        double? aerialDistance = CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        return aerialDistance * 1.3; // מקדם לקיצור או הארכת הדרך הרגלית
    }

    // המרחק לנסיעה (סימולציה בסיסית)
    private static double? SimulatedDrivingDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        double? aerialDistance = CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        return aerialDistance * 1.5; // מקדם להתחשבות בכבישים ומסלולים
    }

    private static double? DegreesToRadians(double? degrees)
    {
        
        return degrees!=null?degrees * (Math.PI / 180):null;
    }

    internal static bool IsValidEmail(string? email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    public static bool IsValidPhoneNumber(string phone)
    {
        // Check if the phone number is numeric and has the correct length (e.g., 10 digits for Israeli numbers)
        return !string.IsNullOrEmpty(phone) && phone.All(char.IsDigit) && phone.Length == 10;
    }

    public static bool AreFieldsNumeric(BO.Volunteer volunteer)
    {

        // checking numeric fields
        return
            int.TryParse(volunteer.Id.ToString(), out _) && // ID חייב להיות מספר
            (volunteer.MaxDistance == null || double.TryParse(volunteer.MaxDistance.ToString(), out _)) && // maxDistance יכול להיות null או מספר
            (volunteer.NumberOfCalls == null || int.TryParse(volunteer.NumberOfCalls.ToString(), out _)) && // sumTreatedCalls יכול להיות null או מספר
            (volunteer.NumberOfCanceledCalls == null || int.TryParse(volunteer.NumberOfCanceledCalls.ToString(), out _)) && // sumCallsSelfCancelled יכול להיות null או מספר
            (volunteer.NumberOfexpiredCalls == null || int.TryParse(volunteer.NumberOfexpiredCalls.ToString(), out _)); // sumExpiredCalls יכול להיות null או מספר
    }

    internal static bool IsValidFieldLength(string? field, int minLength, int maxLength)
    {
        return !string.IsNullOrWhiteSpace(field) && field.Length >= minLength && field.Length <= maxLength;
    }

    internal static void Validation(BO.Volunteer volunteer)
    {
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid Israeli ID.");
        }

        if (!IsValidEmail(volunteer.Email))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid email format.");
        }

        if (!IsValidFieldLength(volunteer.FullName, 2, 50))
        {
            throw new BO.Exceptions.BLInvalidDataException("Full name must be between 2 and 50 characters.");
        }

        if (!IsValidPhoneNumber(volunteer.Phone))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid phone number.");
        }

        if (!AreFieldsNumeric(volunteer))
        {
            throw new BO.Exceptions.BLInvalidDataException("One or more numeric fields in the volunteer object contain invalid data.");
        }
    }
 
    private static int s_periodicCounter = 0;

    internal static void SimulateVolunteerAssignments()
    {
        var callImpl = new CallImplementation();
        var volunteerImpl = new VolunteerImplementation();

        var activeVolunteers = v_dal.Volunteer.ReadAll();
        var rand = new Random();

        foreach (var BoVolunteer in activeVolunteers)
        {
            var volunteer = volunteerImpl.Read(BoVolunteer.Id);
            var currentCallInProgress = volunteer.CurrentCallInProgress;

            // מתנדב פנוי
            if (currentCallInProgress == null)
            {
                int toAssign = rand.Next(1, 11); // 10% סיכוי לשבץ
                if (toAssign == 1)
                {
                    // מחפשים את סוג הקריאה השכיח ביותר של המתנדב בעבר
                    var mostCommonType = callImpl.GetClosedCallsByVolunteer(volunteer.Id)
                        .GroupBy(c => c.CallType)
                        .OrderByDescending(g => g.Count())
                        .Select(g => (BO.Enums.CallType?)g.Key)
                        .FirstOrDefault();

                    IEnumerable<BO.OpenCallInList> openCalls;
                    if (mostCommonType != null)
                        openCalls = callImpl.GetOpenCallsForVolunteer(volunteer.Id,v=>v.CallType== mostCommonType);
                    else
                        openCalls = callImpl.GetOpenCallsForVolunteer(volunteer.Id);

                    openCalls = openCalls.OrderBy(c => c.DistanceFromVolunteer);
                    if (openCalls.Any())
                        CallManager.AssignCallToVolunteer(volunteer.Id, openCalls.First().Id);
                }
            }

            // מתנדב מטפל כעת בקריאה
            else
            {
                DateTime entryTime = currentCallInProgress.EntryTimeToHandle;
                int extraTime = 10 + (int)Math.Floor((currentCallInProgress.DistanceFromVolunteer??0) * 2) + rand.Next(0, 6);

                if (entryTime.AddMinutes(extraTime) <= AdminManager.Now)
                {
                    CallManager.CompleteCallTreatment(volunteer.Id, currentCallInProgress.Id);
                }
                else
                {
                    int toCancel = rand.Next(1, 11); // 10%
                    if (toCancel == 1)
                        CallManager.closeLastAssignmentByCallId(currentCallInProgress.Id, toCancel%2==0?DO.Enums.AssignmentStatus.MANAGER_CANCELLED: DO.Enums.AssignmentStatus.SELF_CANCELLED);
                }
            }
        }
    }

}

