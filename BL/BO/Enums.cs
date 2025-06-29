namespace BO;
public class Enums
{
    public enum Role { volunteer, manager, NONE };
    public enum DistanceType { walkDistance, driveDistance, airDistance };
    public enum TypeOfTreatmentTerm {
        AssignedAndInProgress,
        TREATED,           // The call was treated on time
        SELF_CANCELLED,    // The volunteer canceled the call
        MANAGER_CANCELLED, // The manager canceled the call
        EXPIRED,        // The call expired and was automatically canceled
        NONE
    };
    public enum VolunteerSortField
    {
        ID,                           // Sort by volunteer ID
        FULL_NAME,                    // Sort by full name of the volunteer
        IS_AVAILABLE,                  // Sort by availability status
        SUM_TREATED_CALLS,             // Sort by total number of treated calls
        SUM_CALLS_SELF_CANCELLED,      // Sort by total calls canceled by volunteer
        SUM_EXPIRED_CALLS,             // Sort by total expired calls
        CALL_ID,                       // Sort by call ID
        CALL_TYPE  ,                   // Sort by type of call
        NONE                           //show by default
    }
    //public enum TypeHosting
    //{
    //    TEMPORARY_LIVING,   // Temporary accommodation for volunteers
    //    EMERGENCY_SHELTER,  // Shelter provided during emergencies
    //    SHABBAT_NEAR_HOSPITALS, // Accommodation for volunteers near hospitals during Shabbat
    //    SHORT_TERM_LODGING   // Short-term lodging for volunteers
    //}
    public enum CallInListFields
    {
        Id,                   // Unique identifier for the call
        CallId,               // ID associated with the specific call
        CallType,             // Type of the call (e.g., emergency, temporary living)
        OpeningTime,          // The time the call was opened
        RemainingTime,        // The remaining time for the call to be handled
        LastVolunteerName,    // The name of the last volunteer assigned to the call
        TotalHandlingTime,    // Total time spent handling the call
        CallStatus,           // The current status of the call
        TotalAssignments      // Total number of assignments associated with the call
    }
    public enum ClosedCallInListFields
    {
        Id,                   // Unique identifier for the closed call
        CallType,             // Type of the closed call
        FullAddress,          // Full address where the call was handled
        OpenTime,             // The time the call was opened
        EntryTime,            // The time the volunteer entered to handle the call
        ActualEndTime,        // The actual time the call handling ended
        EndType               // The type of closure (e.g., treated, expired)
    }
    public enum CallType 
    { salad, 
      desert, 
      mainMeal, 
      pastry, 
      NONE 
    };

    public enum CallStatus
    {
        InProgress,         // Call currently being worked on by a volunteer
        OpenAtRisk,             // Call approaching the required handling time
        InProgressAtRisk,   // Call in progress and approaching risk
        Expired,            // Call expired and was canceled
        Closed,             // Call has been successfully closed
        Open                
    }

    public enum FinishType
    {
        Completed,  // Successfully completed
        Canceled,   // Canceled by the volunteer
        Expired,     // Expired without completion
           NONE         // Add more finish types as needed
    }
    /// <summary>
    /// Enum representing different statuses an assignment can have.
    /// </summary>
    public enum AssignmentStatus
    {
        AssignedAndInProgress,
        TREATED,           // The call was treated on time
        SELF_CANCELLED,    // The volunteer canceled the call
        MANAGER_CANCELLED, // The manager canceled the call
        EXPIRED    ,        // The call expired and was automatically canceled
            NONE
    }
    public enum OpenCallInListFields
    {
        None,
        Id,                   // Unique identifier for the call
        CallType,             // The type of the call
        Description,          // A textual description of the call
        FullAddress,          // The full address of the call
        OpenTime,             // The time the call was opened
        MaxEndTime,           // The maximum allowed time to close the call
        DistanceFromVolunteer // The distance of the call from the current location of the volunteer
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