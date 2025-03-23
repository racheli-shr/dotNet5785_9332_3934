using DO;
using static DO.Call;
using System.Numerics;
using Microsoft.VisualBasic;

namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="Id">id of the volunteer</param>
/// <param name="CallType"> </param>
/// <param name="Description"></param>
/// <param name="FullAdress"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="OpeningCallTime"></param>
/// <param name="MaxTimeToEnd"></param>
public record Call
(
    int Id,
    DO.Enums.CallType CallType,  // הגדרת enum עם סוג "CallType"
    string? Description,
    string FullAdress,
    double Latitude,
    double Longitude,
    DateTime OpeningCallTime,
    DateTime? MaxTimeToEnd  // כאן הערך יכול להיות null
)
{
    // בנאי ברירת מחדל שמאתחל את הערכים
    public Call() : this(0, default(DO.Enums.CallType), "No description", "Unknown", 0.0, 0.0, DateTime.Now, null) { }
    public override string ToString()
    {
        return "";
        //        return $@"
        //Volunteer Details:
        //-------------------
        //ID: {Id}
        //Full Name: {fullName}
        //Phone: {phone}
        //Email: {email}
        //Address: {(address ?? "Not Provided")}
        //Role: 
        //        {Role}
        //Availability: {(isAvailable ? "Available" : "Not Available")}
        //Distance Type: {typeDistance}
        //Max Distance: {(maxDistance.HasValue ? $"{maxDistance.Value} km" : "Not Specified")}
        //Location: {(Latitude.HasValue && Longitude.HasValue ? $"({Latitude.Value}, {Longitude.Value})" : "Not Specified")}
        //";
    }
}
