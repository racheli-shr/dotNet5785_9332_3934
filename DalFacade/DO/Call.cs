


using static DO.Volunteer;
using System.Numerics;

namespace DO;

public record Call
{
    int Id;
    enum CallType;
    string Description;
    string FullAdress;
    double Latitude;
    double Longitude;
    DateTime OpeningCallTime;
    DateTime MaxTimeToEnd;


    public Call(){ }

}

