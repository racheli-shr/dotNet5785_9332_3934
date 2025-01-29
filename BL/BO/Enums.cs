namespace BO;
public class Enums
{
    public enum Role { volunteer, manager };
    public enum DistanceType { walkDistance, driveDistance, airDistance };
    public enum TypeOfTreatmentTerm { finished, selfCancelation, managerCancelation, endTermCancelation };
    public enum VolunteerSortField
    {
        ID,                           // Sort by volunteer ID
        FULL_NAME,                    // Sort by full name of the volunteer
        IS_AVAILABLE,                  // Sort by availability status
        SUM_TREATED_CALLS,             // Sort by total number of treated calls
        SUM_CALLS_SELF_CANCELLED,      // Sort by total calls canceled by volunteer
        SUM_EXPIRED_CALLS,             // Sort by total expired calls
        CALL_ID,                       // Sort by call ID
        CALL_TYPE                      // Sort by type of call
    }
    public enum CallType
    {
        Emergency, // Urgent call
        Regular,   // Regular call
        FollowUp   // Follow-up call
                   // Add more types as needed
    }

    public enum CallStatus
    {
        InProgress,         // Call currently being worked on by a volunteer
        AtRisk,             // Call approaching the required handling time
        InProgressAtRisk,   // Call in progress and approaching risk
        Expired,            // Call expired and was canceled
        Closed,             // Call has been successfully closed
        Open                
    }

    public enum FinishType
    {
        Completed,  // Successfully completed
        Canceled,   // Canceled by the volunteer
        Expired     // Expired without completion
                    // Add more finish types as needed
    }
    public enum AssignmentEndType
    {
        Completed,       // The call handling was successfully completed
        Cancelled,       // The call was cancelled
        Expired,         // The call expired without handling
        ArtificialCancel // Artificially created assignment due to expiration or other reasons
                         // Add more end types as needed
    }

    
    public enum TimeUnit
    {
        MINUTE, // Minute unit
        HOUR,   // Hour unit
        DAY,    // Day unit
        MONTH,  // Month unit
        YEAR    // Year unit
    }
}