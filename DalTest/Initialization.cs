namespace DalTest;
using DalApi;
using DO;
using System.Data;
using System.Security.Principal;

public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    private static void createAssignment()
    {
        for (int i = 0; i < 10; i++)
        {
            List<Volunteer> VolList = s_dal.Volunteer.ReadAll().ToList();
            int CallId = s_rand.Next(1000, 100000);
            int VolunteerId = VolList[s_rand.Next(0, VolList.Count)].Id;
            DateTime EntryTimeForTreatment = s_dal!.Config.Clock;
            DateTime? ActualTreatmentEndTime = s_dal!.Config.Clock.AddDays(14);
            TypeOfTreatmentTerm? TypeOfTreatmentTermination = i % 2 == 0 ? i % 4 == 0 ? TypeOfTreatmentTerm.endTermCancelation : TypeOfTreatmentTerm.selfCancelation : i % 3 == 0 ? TypeOfTreatmentTerm.selfCancelation : TypeOfTreatmentTerm.finished;
            s_dal!.Assignment.Create(new(0, CallId, VolunteerId, EntryTimeForTreatment, ActualTreatmentEndTime, TypeOfTreatmentTermination));
        }
    }
    private static void createVolunteer()
    {
        int MIN_ID = 200000000;
        int MAX_ID = 400000000;
        int MIN_PHONE = 64000000;
        int MAX_PHONE = 65999999;

        string[] fullName = { "Yael Bar", "Racheli Tal ", "Hadas Shay", "Shira Or", "Daniel Zohar", "David Mor" };
        string[] adress = { "mango 32", "orange 15", "ananas 66", "kiwi 46", "apple 21", "cherry 10" };

        int i = 0;
        foreach (string name in fullName)
        {
            int Id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dal!.Volunteer.Read(a => a.Id == Id) != null)
            {
                Id = s_rand.Next(MIN_ID, MAX_ID);
            }
            string FullName = name;
            int phoneNumber = s_rand.Next(MIN_PHONE, MAX_PHONE);
            string phone = $"02-{phoneNumber}";
            string email = $"{name.Trim().Replace(" ", "")}@gmail.com";
            string password = s_dal!.Volunteer.GenerateStrongPassword();
            string encriptedPassword = s_dal!.Volunteer.EncryptPassword(password);
            string fullAdress = $"{adress[i]}";
            double longtitude = (s_rand.NextDouble() * 360) - 180;
            double latitude = (s_rand.NextDouble() * 180) - 90;
            Role role = (i == 0 ? Role.manager : Role.volunteer);
            bool isIative = (i % 2 == 0) ? false : true;
            double maxDistance = i * i + 50;
            DistanceType distanceType = (i % 2 == 0 ? DistanceType.walkDistance : i % 3 == 0 ? DistanceType.airDistance : DistanceType.driveDistance);
            s_dal!.Volunteer.Create(new(Id, FullName, phone, email, encriptedPassword, fullAdress, latitude, longtitude, role, isIative, maxDistance, distanceType));
            i += 1;

        }
    }
    private static void createCall()
    {
        string[] adress = { "mango 66", "orange 56", "ananas 12", "kiwi 88", "apple 2", "cherry 3" };
        string[] sortOfFood = { "I want to prepare a lemomCake", "Hi mazal tov! i prepare a creamy champinion pasta", "harbe nahat! i bring a cheesy salad", "oh a new baby! i bring the family a chocolate pai" };
        for (int i = 0; i < 4; i++)
        {
            CallType CallType = i % 2 == 0 ? i % 4 == 0 ? CallType.desert : CallType.mainMeal : i % 3 == 0 ? CallType.salade : CallType.pastry;
            string? Description = sortOfFood[i];
            string FullAdress = adress[i + 1];
            double Latitude = (s_rand.NextDouble() * 180) - 90;
            double Longitude = (s_rand.NextDouble() * 180) - 90;
            DateTime OpeningCallTime = s_dal!.Config.Clock.AddDays(i).AddHours(i % 3);
            DateTime? MaxTimeToEnd = s_dal!.Config.Clock.AddDays(i + 14);
            s_dal!.Call.Create(new(0, CallType, Description, FullAdress, Latitude, Longitude, OpeningCallTime, MaxTimeToEnd));
        }
    }
    public static void Do(IDal dal)
    {
        s_dal = dal;
        // הצבת הפרמטרים למשתנים הסטטיים
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2

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
