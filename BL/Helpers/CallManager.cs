using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace BL.Helpers
namespace Helpers;

internal static class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 

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
    private static readonly IDal s_dal = Factory.Get; //stage 4
    static internal BO.Enums.CallStatus GetCallStatus(int id)
    {

        // Retrieve the call from the database or throw an exception if not found
        DO.Call call = s_dal.Call.Read(v => v.Id == id)
            ?? throw new BO.Exceptions.BlDoesNotExistException($"Call with ID={id} does not exist");

        // Retrieve the assignment associated with the call (null if not assigned yet)
        DO.Assignment? assignment = s_dal.Assignment.ReadAll(a => a.CallId == call.Id)
            .OrderByDescending(assignment => assignment.EntryTimeForTreatment).FirstOrDefault();

        // Get the current time
        DateTime currentTime = AdminManager.Now;

        // Get the risk time span from the configuration
        TimeSpan riskTimeSpan = ConfigManager.GetRiskTimeSpanFromDal();

        // Calculate the risk threshold time
        DateTime? riskThreshold = call.MaxTimeToEnd?.Subtract(riskTimeSpan);

        // Case 1: Call is not assigned and no one is handling it
        if (assignment == null)
        {
            if (call.MaxTimeToEnd != null && currentTime > call.MaxTimeToEnd)
            {
                return BO.Enums.CallStatus.Expired; // Call expired without being handled
            }
            if (riskThreshold != null && currentTime >= riskThreshold)
            {
                return BO.Enums.CallStatus.OpenAtRisk; // Call is at risk but still open
            }
            return BO.Enums.CallStatus.Open; // Call is open and not yet at risk
        }

        // Case 2: Call has an assignment, determine status based on AssignmentStatus
        switch (assignment.AssignmentStatus)
        {
            case DO.Enums.AssignmentStatus.TREATED:
                return BO.Enums.CallStatus.Closed; // Call was treated successfully

            case DO.Enums.AssignmentStatus.SELF_CANCELLED:
            case DO.Enums.AssignmentStatus.MANAGER_CANCELLED:
                return BO.Enums.CallStatus.Open; // Call was reopened after cancellation

            case DO.Enums.AssignmentStatus.EXPIRED:
                return BO.Enums.CallStatus.Expired; // Call expired without being handled
        }

        // Case 3: Call is in progress
        if (call.MaxTimeToEnd != null)
        {
            if (currentTime > call.MaxTimeToEnd)
            {
                return BO.Enums.CallStatus.Expired; // Call expired during progress
            }
            if (currentTime >= riskThreshold)
            {
                return BO.Enums.CallStatus.InProgressAtRisk; // Call is in progress and at risk
            }
            return BO.Enums.CallStatus.InProgress; // Call is in progress and not at risk
        }

        // Default: If no specific case applies
        return BO.Enums.CallStatus.Open;
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
        var lastAssignment = s_dal.Assignment
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
            DO.Enums.AssignmentStatus.EXPIRED => BO.Enums.CallStatus.Expired,
            _ or null => BO.Enums.CallStatus.InProgress
        };
    }





}
