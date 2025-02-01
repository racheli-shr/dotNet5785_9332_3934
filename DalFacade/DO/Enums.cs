namespace DO;

public class Enums
{
    public enum Role { volunteer, manager };
    public enum DistanceType { walkDistance, driveDistance, airDistance };
    public enum TypeOfTreatmentTerm { finished, selfCancelation, managerCancelation, endTermCancelation };
    public enum CallType { salade, desert, mainMeal, pastry };
    public enum TypeHosting
    {
        TEMPORARY_LIVING,   // Temporary accommodation for volunteers
        EMERGENCY_SHELTER,  // Shelter provided during emergencies
        SHABBAT_NEAR_HOSPITALS, // Accommodation for volunteers near hospitals during Shabbat
        SHORT_TERM_LODGING   // Short-term lodging for volunteers
    }
}