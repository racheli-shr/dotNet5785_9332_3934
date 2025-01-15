namespace BO;

public enum Role { volunteer, manager };
public enum DistanceType { walkDistance, driveDistance, airDistance };
public enum TypeOfTreatmentTerm { finished, selfCancelation, managerCancelation, endTermCancelation };

public enum CallType
{
    Emergency, // Urgent call
    Regular,   // Regular call
    FollowUp   // Follow-up call
               // Add more types as needed
}

public enum CallStatus
{
    InProgress,          // Currently being handled
    AtRiskInProgress     // Being handled with risks
                         // Add more statuses as needed
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

public enum VolunteerSortField
{
    Id,
    Name,
    Role
}

public enum TimeUnit
{
    MINUTE, // Minute unit
    HOUR,   // Hour unit
    DAY,    // Day unit
    MONTH,  // Month unit
    YEAR    // Year unit
}