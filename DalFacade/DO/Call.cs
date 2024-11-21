


using static DO.Volunteer;
using System.Numerics;

namespace DO;

public record Call
{
    public int Id;
    public enum CallType;
    public string Description { get; set; }
    public string FullAdress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpeningCallTime { get; set; }
    public DateTime MaxTimeToEnd { get; set; }


    public Call()
    {
        FullAdress = "Unknown";
        Latitude = 0.0;
        Longitude = 0.0;
        //CallType = null;     לא הוגדר
        Description = "No description";
        OpeningCallTime = DateTime.Now;
        MaxTimeToEnd = DateTime.Now.AddHours(1);
    }

}

