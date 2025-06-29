namespace DalTest;
using DalApi;
using DO;
using System.Data;
using System.Security.Principal;
using static DO.Exceptions;

public static class Initialization
{
    //initialization of data
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    private static void createAssignment()
    {
        List<Volunteer> VolList = s_dal.Volunteer.ReadAll().ToList();
        List<Call> CallList = s_dal.Call.ReadAll().ToList();
        for (int i = 0; i < 10; i++)
        {

            int CallId = CallList[s_rand.Next(0, CallList.Count)].Id;
            int VolunteerId = VolList[s_rand.Next(0, VolList.Count)].Id;
            DateTime EntryTimeForTreatment = s_dal!.Config.Clock;
            DateTime? ActualTreatmentEndTime = s_dal!.Config.Clock.AddDays(7);
            DO.Enums.AssignmentStatus assignmentStatus = CallId%3==0? DO.Enums.AssignmentStatus.TREATED:CallId%2==0? DO.Enums.AssignmentStatus.MANAGER_CANCELLED:CallId%1==0? DO.Enums.AssignmentStatus.SELF_CANCELLED: DO.Enums.AssignmentStatus.EXPIRED;
            s_dal!.Assignment.Create(new(0, CallId, VolunteerId, EntryTimeForTreatment, ActualTreatmentEndTime,assignmentStatus));
        }
    }
    private static void createVolunteer()
    {
        int MIN_ID = 20000000;
        int MAX_ID = 40000000;
        int MIN_PHONE = 500000000;
        int MAX_PHONE = 599999999;

        string[] fullName = { "Yael Bar", "Racheli Tal ", "Hadas Shay", "Shira Or", "Daniel Zohar", "David Mor" };
        string[] adress = { "mango 32", "orange 15", "ananas 66", "kiwi 46", "apple 21", "cherry 10" };

        int i = 0;
        foreach (string name in fullName)
        {
            
            string FullName = name;
            int phoneNumber = s_rand.Next(MIN_PHONE, MAX_PHONE);
            string phone = $"0{phoneNumber}";
            string email = $"{name.Trim().Replace(" ", "")}@gmail.com";
            string password = s_dal!.Volunteer.GenerateStrongPassword();
            string encriptedPassword = s_dal!.Volunteer.EncryptPassword(password);
            string fullAdress = $"{adress[i]}";
            double longtitude = (s_rand.NextDouble() * 360) - 180;
            double latitude = (s_rand.NextDouble() * 180) - 90;
            DO.Enums.Role role = (i == 0 ? DO.Enums.Role.manager : DO.Enums.Role.volunteer);
            bool isIative = (i % 2 == 0) ? false : true;
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
        string[] adress = { "mango 66", "orange 56", "ananas 12", "kiwi 88", "apple 2", "cherry 3" };
        string[] sortOfFood = { "I want to prepare a lemomCake", "Hi mazal tov! i prepare a creamy champinion pasta", "harbe nahat! i bring a cheesy salad", "oh a new baby! i bring the family a chocolate pai" };
        for (int i = 0; i < 4; i++)
        {
            DO.Enums.CallType CallType = i % 2 == 0 ? i % 4 == 0 ? DO.Enums.CallType.desert : DO.Enums.CallType.mainMeal : i % 3 == 0 ? DO.Enums.CallType.salad : DO.Enums.CallType.pastry;
            string? Description = sortOfFood[i];
            string FullAdress = adress[i + 1];
            double Latitude = (s_rand.NextDouble() * 180) - 90;
            double longtitude = (s_rand.NextDouble() * 180) - 90;
            DateTime OpeningCallTime = s_dal!.Config.Clock.AddDays(i).AddHours(i % 3);
            DateTime? MaxTimeToEnd = s_dal!.Config.Clock.AddDays(i + 14);
            s_dal!.Call.Create(new(0, CallType, Description, FullAdress, Latitude, longtitude, OpeningCallTime, MaxTimeToEnd));
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
