namespace DO;

public class Enums
{
    public enum Role { volunteer, manager };
    public enum DistanceType { walkDistance, driveDistance, airDistance };
    
    public enum CallType { salade, desert, mainMeal, pastry };
    /// <summary>
    /// Enum representing different statuses an assignment can have.
    /// </summary>
    public enum AssignmentStatus
    {
        TREATED,           // The call was treated on time
        SELF_CANCELLED,    // The volunteer canceled the call
        MANAGER_CANCELLED, // The manager canceled the call
        EXPIRED  ,          // The call expired and was automatically canceled
    NONE
    }
}