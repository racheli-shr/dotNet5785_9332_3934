
using System.Data;
using static DO.Volunteer;

namespace DO;

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
    Role Role,  // Role הוא Enum
    bool IsActive,
    double? MaxDistance,
    DistanceType? DistanceType  // DistanceType הוא Enum, Nullable
)
{
    // אם ברצונך להוסיף בנאי ברירת מחדל עם ערכים, אפשר להגדיר כך:
    public Volunteer() : this(0, "Unknown", "Unknown", "Unknown", null, "Unknown", null, null, Role.volunteer, true, null, null) { }
}