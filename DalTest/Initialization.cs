namespace DalTest;
using DalApi;
using DO;
using System.Data;
using System.Numerics;
using System.Security.Principal;
using static DO.Enums;
using static DO.Exceptions;

public static class Initialization
{
    //initialization of data
    private static IDal? s_dal;
    private static readonly Random s_rand = new();
    private static void createVolunteer()
    {
        s_dal.Volunteer!.Create(new(328183934, "Yael Bloch", "0534183542", "yaelbloch2023@gmail.com", s_dal!.Volunteer.EncryptPassword("Yael@202") , "moshe zilberg 32", 31.762107245509604, 35.1859516513024, DO.Enums.Role.manager, true, 100, DO.Enums.DistanceType.driveDistance));
        s_dal.Volunteer!.Create(new(321226227, "Lea Bloch", "0548440911", "lea@smileart.co.il", s_dal!.Volunteer.EncryptPassword("Yael@202") , "moshe zilberg 30", 31.76146873975337, 35.18567807502385, DO.Enums.Role.volunteer, true, 100, DO.Enums.DistanceType.airDistance));
        int MIN_ID = 20000000;
        int MAX_ID = 40000000;
        int MIN_PHONE = 500000000;
        int MAX_PHONE = 599999999;

        string[] fullName = { "Yael Bar", "Racheli Tal ", "Hadas Shay", "Shira Or", "Daniel Zohar", "David Mor" };
        var address = DalTest.AddAddress.GetAddAddresses();
        int i = 0;
        foreach (string name in fullName)
        {
            
            string FullName = name;
            int phoneNumber = s_rand.Next(MIN_PHONE, MAX_PHONE);
            string phone = $"0{phoneNumber}";
            string email = $"{name.Trim().Replace(" ", "")}@gmail.com";
            string password = s_dal!.Volunteer.GenerateStrongPassword();
            string encriptedPassword = s_dal!.Volunteer.EncryptPassword(password);
            string fullAdress = $"{address[i].StringAddress}";
            double longtitude = address[i].Longitude;
            double latitude = address[i].Latitude;
            DO.Enums.Role role =  DO.Enums.Role.volunteer;
            bool isIative =(i % 2 == 0) ? false : true;
            double maxDistance = i * i + 50;
            DO.Enums.DistanceType distanceType = (i % 2 == 0 ? DO.Enums.DistanceType.walkDistance : i % 3 == 0 ? DO.Enums.DistanceType.airDistance : DO.Enums.DistanceType.driveDistance);
            int Id;
            do
            {
                Id = GetValidIsraeliID(s_rand.Next(MIN_ID, MAX_ID));
                try
                {
                    s_dal.Volunteer!.Read(Id); // Check if a volunteer with this ID already exists
                }
                catch (DalDoesNotExistException) // If the ID does not exist, it's available
                {
                    break; // Exit the loop since we found a free ID
                }
            } while (true);


            
            s_dal.Volunteer!.Create(new(Id, FullName, phone, email, encriptedPassword, fullAdress, latitude, longtitude, role, isIative, maxDistance, distanceType));
            i += 1;

        }
    }
    private static void createCall()
    {
        var address = DalTest.AddAddress.GetAddAddresses();
        string[] sortOfFood = { "I want to prepare a lemomCake", "Hi mazal tov! i prepare a creamy champinion pasta", "harbe nahat! i bring a cheesy salad", "oh a new baby! i bring the family a chocolate pai" };
        for (int i = 0; i < 15; i++)
        {
            DO.Enums.CallType CallType = i % 2 == 0 ? i % 4 == 0 ? DO.Enums.CallType.desert : DO.Enums.CallType.mainMeal : i % 3 == 0 ? DO.Enums.CallType.salad : DO.Enums.CallType.pastry;
            string? Description = sortOfFood[i%4];
            string FullAdress = address[i + 1].StringAddress;
            double? Latitude = address[i+1].Latitude;
            double? longtitude = address[i+1].Longitude;
            DateTime OpeningCallTime = s_dal!.Config.Clock.AddDays(i).AddHours(i % 3);
            DateTime? MaxTimeToEnd = s_dal!.Config.Clock.AddDays(i + 14);
            s_dal!.Call.Create(new(0, CallType, Description, FullAdress, Latitude, longtitude, OpeningCallTime, MaxTimeToEnd));
        }
    }

    private static void createAssignment()
    {
        // סינון רק מתנדבים פעילים
        List<Volunteer> VolList = s_dal.Volunteer.ReadAll()
            .Where(v => v.IsActive)
            .ToList();

        if (VolList.Count == 0)
            throw new InvalidOperationException("No active volunteers available.");

        List<Call> CallList = s_dal.Call.ReadAll().ToList();

        for (int i = 0; i < 10 && i < CallList.Count; i++)
        {
            int CallId = CallList[i].Id;

            // הגרלת מתנדב פעיל
            int VolunteerId = VolList[s_rand.Next(VolList.Count)].Id;

            DateTime EntryTimeForTreatment = s_dal!.Config.Clock;
            DateTime? ActualTreatmentEndTime = EntryTimeForTreatment.AddYears(1);

            // קביעה דינמית של סטטוס
            DO.Enums.AssignmentStatus assignmentStatus =
                CallId % 4 == 0 ? DO.Enums.AssignmentStatus.TREATED :
                CallId % 3 == 0 ? DO.Enums.AssignmentStatus.MANAGER_CANCELLED :
                CallId % 2 == 0 ? DO.Enums.AssignmentStatus.SELF_CANCELLED :
                DO.Enums.AssignmentStatus.AssignedAndInProgress;

            s_dal!.Assignment.Create(new(
                0,
                CallId,
                VolunteerId,
                EntryTimeForTreatment,
                ActualTreatmentEndTime,
                assignmentStatus));
        }
    }


    public static int GetValidIsraeliID(int id)
    {
        if (id < 10000000 || id > 99999999)
            throw new ArgumentException("ID must contain 8 digits");

        int sum = 0;
        int tempId = id;

        for (int i = 7; i >= 0; i--)
        {
            int digit = tempId % 10;
            int factor = (i % 2 == 0) ? 1 : 2;
            int product = digit * factor;
            sum += (product > 9) ? product - 9 : product;
            tempId /= 10;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        return id * 10 + checkDigit;
    }
    public static void Do()
    {
        //s_dal = dal;
        // הצבת הפרמטרים למשתנים הסטטיים
        s_dal = DalApi.Factory.Get; //stage 4


        // איפוס נתוני התצורה ואיפוס רשימות נתונים
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();
        // קריאה לפונקציות אתחול הרשימות
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteer();

        Console.WriteLine("Initializing Calls list ...");
        createCall();

        Console.WriteLine("Initializing Assignments list ...");
        createAssignment();
    }
}
