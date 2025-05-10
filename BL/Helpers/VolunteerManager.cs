
using BL.Helpers;
using DalApi;
using System.ComponentModel;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal v_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5
    // Define the delegate for converting BO.Volunteer to DO.Volunteer
    public static DO.Volunteer ConvertBOToDO(BO.Volunteer input) =>
        new DO.Volunteer
        {
            Id = input.Id,                    // Mapping Id
            FullName = input.FullName,        // Mapping fullName (ensure casing matches)
            Phone = input.Phone,              // Mapping phone
            Email = input.Email,              // Mapping email
            FullAdress = input.FullAddress,          // Mapping address                            
            // Password = input.Password,     // Uncomment if needed
            IsActive = input.IsActive,  // Mapping isAvailable
            Role = (DO.Enums.Role)input.Role,                // Mapping Role
            DistanceType = (DO.Enums.DistanceType)input.DistanceType, // Mapping typeDistance
            MaxDistance = input.MaxDistance,  // Mapping maxDistance
            Latitude = input.Latitude,        // Mapping Latitude
            Longitude = input.Longitude       // Mapping Longitude
        };
    internal static bool IsValidIsraeliID(string id)
    {
        if (id.Length != 9 || !long.TryParse(id, out _))
            return false;
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = id[i] - '0'; //מחזיר לי את הערך המספרי של התו
            if (i % 2 == 0)
                sum += digit;
            else
            {
                int doubled = digit * 2;
                sum += (doubled > 9) ? doubled - 9 : doubled;
            }
        }
        int checksum = (10 - (sum % 10)) % 10;
        return checksum == (id[8] - '0');
    }

    internal static double GetDistance(double volLatitude, double volLongitude, double callLatitude, double callLongitude, DO.Enums.DistanceType? typeDistance)
    {

        switch (typeDistance)
        {
            case DO.Enums.DistanceType.airDistance:
                return CalculateHaversineDistance(volLatitude, volLongitude, callLatitude, callLongitude);

            case DO.Enums.DistanceType.walkDistance:
                return SimulatedWalkingDistance(volLatitude, volLongitude, callLatitude, callLongitude);

            case DO.Enums.DistanceType.driveDistance:
                return SimulatedDrivingDistance(volLatitude, volLongitude, callLatitude, callLongitude);

            default:
                return 0.0;
        }

    }

    // חישוב מרחק אווירי נוסחת Haversine
    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // רדיוס כדור הארץ בק"מ
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // המרחק בקילומטרים
    }

    // המרחק להליכה (סימולציה בסיסית)
    private static double SimulatedWalkingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double aerialDistance = CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        return aerialDistance * 1.3; // מקדם לקיצור או הארכת הדרך הרגלית
    }

    // המרחק לנסיעה (סימולציה בסיסית)
    private static double SimulatedDrivingDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double aerialDistance = CalculateHaversineDistance(lat1, lon1, lat2, lon2);
        return aerialDistance * 1.5; // מקדם להתחשבות בכבישים ומסלולים
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    internal static bool IsValidEmail(string? email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    public static bool IsValidPhoneNumber(string phone)
    {
        // Check if the phone number is numeric and has the correct length (e.g., 10 digits for Israeli numbers)
        return !string.IsNullOrEmpty(phone) && phone.All(char.IsDigit) && phone.Length == 10;
    }

    public static bool AreFieldsNumeric(BO.Volunteer volunteer)
    {

        // checking numeric fields
        return
            int.TryParse(volunteer.Id.ToString(), out _) && // ID חייב להיות מספר
            (volunteer.MaxDistance == null || double.TryParse(volunteer.MaxDistance.ToString(), out _)) && // maxDistance יכול להיות null או מספר
            (volunteer.NumberOfCalls == null || int.TryParse(volunteer.NumberOfCalls.ToString(), out _)) && // sumTreatedCalls יכול להיות null או מספר
            (volunteer.NumberOfCanceledCalls == null || int.TryParse(volunteer.NumberOfCanceledCalls.ToString(), out _)) && // sumCallsSelfCancelled יכול להיות null או מספר
            (volunteer.NumberOfexpiredCalls == null || int.TryParse(volunteer.NumberOfexpiredCalls.ToString(), out _)); // sumExpiredCalls יכול להיות null או מספר
    }

    internal static bool IsValidFieldLength(string? field, int minLength, int maxLength)
    {
        return !string.IsNullOrWhiteSpace(field) && field.Length >= minLength && field.Length <= maxLength;
    }

    internal static void Validation(BO.Volunteer volunteer)
    {
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid Israeli ID.");
        }

        if (!IsValidEmail(volunteer.Email))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid email format.");
        }

        if (!IsValidFieldLength(volunteer.FullName, 2, 50))
        {
            throw new BO.Exceptions.BLInvalidDataException("Full name must be between 2 and 50 characters.");
        }

        if (!IsValidPhoneNumber(volunteer.Phone))
        {
            throw new BO.Exceptions.BLInvalidDataException("Invalid phone number.");
        }

        if (!AreFieldsNumeric(volunteer))
        {
            throw new BO.Exceptions.BLInvalidDataException("One or more numeric fields in the volunteer object contain invalid data.");
        }
    }
//    internal static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock) //stage 4
//    {
//        var list = v_dal.Volunteer.ReadAll().ToList();
//        foreach (var doVolunteer in list)
//        {
//            //if student study for more than MaxRange years
//            //then student should be automatically updated to 'not active'
//            if (AdminManager.Now.Year - doVolunteer.registrationDate?.Year >= AdminManager.MaxRange)
//            {
//                v_dal.Volunteer.Update(doVolunteer with { IsActive = false });
//            }
//        }
//    }
}
