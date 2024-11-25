


using static DO.Volunteer;
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
    CallType CallType,  // הגדרת enum עם סוג "CallType"
    string? Description,
    string FullAdress,
    double Latitude,
    double Longitude,
    DateTime OpeningCallTime,
    DateTime? MaxTimeToEnd
)
{
    // בנאי ברירת מחדל שמאתחל את הערכים
    public Call() : this(0, "No description", "Unknown", 0.0, 0.0, DateTime.Now, DateTime.Now.AddHours(1)) { }
}

