
using static DO.Volunteer;

namespace DO;

public record Volunteer
{
	int Id;
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? FullAdress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public enum Role;
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public enum DistanseType;

    public Volunteer()
    {
        
        FullName = "Unknown";
        Phone = "Unknown";
        Email = "Unknown";
        Password = "Unknown";
        FullAdress = "Unknown";
        Latitude = null;
        Longitude = null;
        //Role = null;      לא מוגדר
        IsActive = true;
        MaxDistance = null;
        //DistanceType = null;   לא מוגדר 
    }
}
