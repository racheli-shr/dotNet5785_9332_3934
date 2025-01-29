
using System.Data;
using static DO.Volunteer;
using DO; // כאן אנחנו מייבאים את ה-namespace שבו נמצאים ה-enums
namespace DO
{
    public record Volunteer
    (
        
        int Id,
        string FullName,
        string Phone,
        string Email,
        string? Password,
        string? FullAdress,
        double? Latitude,
        double? Longitude,
        DO.Enums.Role Role,  // שימוש ב-enum Role
        bool IsActive,
        double? MaxDistance,
        DO.Enums.DistanceType? DistanceType  // שימוש ב-enum DistanceType
    )
    {
        public Volunteer() : this(0, "Unknown", "Unknown", "Unknown", null, null, null, null, DO.Enums.Role.volunteer, true, null, null) { }
    }
}
