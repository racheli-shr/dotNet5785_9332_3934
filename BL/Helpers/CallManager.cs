using DalApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace BL.Helpers
namespace Helpers
{
    internal static class CallManager
    {
        internal static DO.Call ConvertBOToDO(BO.Call call) =>
    new DO.Call
    {
        Id=call.Id,
        CallType=(DO.Enums.CallType)call.CallType,
        FullAdress=call.Address!,
        Latitude=call.Latitude,
        Longitude=call.Longitude,
        OpeningCallTime=call.OpeningTime,
        Description = call.Description,
        MaxTimeToEnd=call.MaxFinishTime
    };
        private static readonly IDal s_dal = Factory.Get; //stage 4
        static internal BO.Enums.CallStatus GetCallStatus(int id)
        {
            // Retrieve the call from the database or throw an exception if not found
            DO.Call call = s_dal.Call.Read(v=>v.Id==id)
                ?? throw new BO.Exceptions.BlDoesNotExistException($"Call with ID={id} does not exist");

            // Retrieve the assignment associated with the call (null if not assigned yet)
            DO.Assignment? assignment = s_dal.Assignment.Read(a => a.CallId == call.Id);

            // Get the current time
            DateTime currentTime = ClockManager.Now;

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
                    return BO.Enums.CallStatus.AtRisk; // Call is at risk but still open
                }
                return BO.Enums.CallStatus.Open; // Call is open and not yet at risk
            }

            // Case 2: Call has an assignment, determine status based on AssignmentStatus
            switch (assignment.TypeOfTreatmentTermination)
            {
                case DO.Enums.TypeOfTreatmentTerm.finished:
                    return BO.Enums.CallStatus.Closed; // Call was treated successfully

                case DO.Enums.TypeOfTreatmentTerm.selfCancelation:
                case DO.Enums.TypeOfTreatmentTerm.managerCancelation:
                    return BO.Enums.CallStatus.Open; // Call was reopened after cancellation

                case DO.Enums.TypeOfTreatmentTerm.endTermCancelation:
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
    }
}
