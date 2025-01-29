namespace DalTest
//{
//    
//}
{
    using System;
   
    using DalApi;
    using DO;
    using Dal;
    internal class Program
    {
        //static readonly IDal s_dal = new DalList(); //stage 2
        //static readonly IDal s_dal = new DalXml(); //stage 3
        static readonly IDal s_dal = Factory.Get; //stage 4
        public static void Main(string[] args)
        {
            try
            {
                MainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Main Menu
        private static void MainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Volunteer Menu");
                Console.WriteLine("2. Call Menu");
                Console.WriteLine("3. Assignment Menu");
                Console.WriteLine("4. Config Menu");
                Console.WriteLine("5. Initialize Data");
                Console.WriteLine("6. Show All Data");
                Console.WriteLine("7. Reset Data");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        VolunteerMenu();
                        break;
                    case "2":
                        CallMenu();
                        break;
                    case "3":
                        AssignmentMenu();
                        break;
                    case "4":
                        ConfigMenu();
                        break;
                    case "5":
                        Initialization.Do();
                        break;
                    case "6":
                        ShowAllData();
                        break;
                    case "7":
                        ResetData();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        //Volunteer Menu
        private static void VolunteerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Volunteer Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Add New Volunteer");
                Console.WriteLine("2. Display Volunteer by ID");
                Console.WriteLine("3. Display All Volunteers");
                Console.WriteLine("4. Update Volunteer");
                Console.WriteLine("5. Delete Volunteer by ID");
                Console.WriteLine("6. Delete All Volunteers");
                Console.WriteLine("7. Reset a volunteer password");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        AddVolunteer();
                        break;
                    case "2":
                        DisplayVolunteerById();
                        break;
                    case "3":
                        DisplayAllVolunteers();
                        break;
                    case "4":
                        UpdateVolunteer();
                        break;
                    case "5":
                        DeleteVolunteerById();
                        break;
                    case "6":
                        DeleteAllVolunteers();
                        break;
                    case "7":
                        updatePasssword();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        //update the Passsword
        private static void updatePasssword()
        {

            Console.WriteLine("enter the id of the volunteer to update his password");
            int id = int.Parse(Console.ReadLine());
            while (s_dal.Volunteer.Read(a => a.Id == id) == null)
            {
                Console.WriteLine("Id is not existing please r-enter the id of the volunteer to update his password");
                id = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
            string password = Console.ReadLine();
            while (!s_dal.Volunteer.checkPassword(password))
            {
                Console.WriteLine("Sorry! Password is incorrect or not strong enough please enter again");
                Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                password = Console.ReadLine();
            }
            string encriptedPassword = s_dal.Volunteer.EncryptPassword(password);
            s_dal.Volunteer.updatePassword(id, encriptedPassword);
            Console.WriteLine("password updated successfully!");
        }
        //Add a new Volunteer
        private static void AddVolunteer()
        {

            try
            {
                // קליטת נתונים מהמסך
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer Name: ");
                string name = Console.ReadLine();
                Console.Write("Enter House Phone Number: ");
                int phone = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer Email: ");
                string email = Console.ReadLine();
                //Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                string password = s_dal.Volunteer.GenerateStrongPassword();//הגרלת סיסמא ראשונית
                Console.WriteLine($"Your password is: {password} you could change it at the volunteer menu.");
                string encriptedPassword = s_dal.Volunteer.EncryptPassword(password);
                Console.Write("Enter Volunteer Adress: ");
                string adress = Console.ReadLine();
                Console.Write("Enter Volunteer latitude a number between -90 to 90: ");
                int latitude = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer longtitude a number between -180 to 180: ");
                int longtitude = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer maxDistance in km: ");
                int maxDistance = int.Parse(Console.ReadLine());
                // יצירת אובייקט חדש והוספתו דרך הממשק
                var volunteer = new Volunteer(id, name, $"02-{phone}", email, encriptedPassword, adress, latitude, longtitude, DO.Enums.Role.volunteer, true, maxDistance, DO.Enums.DistanceType.airDistance);
                s_dal.Volunteer?.Create(volunteer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display a the Volunteer By Id
        private static void DisplayVolunteerById()
        {
            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                var volunteer = s_dal.Volunteer?.Read(a => a.Id == id);
                Console.WriteLine(volunteer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display All the Volunteers
        private static void DisplayAllVolunteers()
        {
            try
            {
                var volunteers = s_dal.Volunteer?.ReadAll();
                foreach (var volunteer in volunteers)
                {
                    Console.WriteLine(volunteer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Update a Volunteer
        private static void UpdateVolunteer()
        {
            try
            {

                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                var volunteer = s_dal.Volunteer?.Read(a => a.Id == id);
                if (volunteer != null)
                {
                    Console.Write("Enter Volunteer Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter House Phone Number: ");
                    int phone = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter a strong password of 8 characters with at least 1 digits from theDecimal digits ,one lowercase Letter, one uppercase lette:");
                    string password = Console.ReadLine();
                    while (!s_dal.Volunteer.checkPassword(password))
                    {
                        Console.WriteLine("Sorry! Password is incorrect or not strong enough please enter again");
                        Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                        password = Console.ReadLine();
                    }
                    string encriptedPassword = s_dal.Volunteer.EncryptPassword(password);
                    Console.Write("Enter Volunteer Adress: ");
                    string adress = Console.ReadLine();
                    Console.Write("Enter Volunteer latitude a number between -90 to 90: ");
                    int latitude = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer longtitude a number between -180 to 180: ");
                    int longtitude = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer maxDistance in km: ");
                    int maxDistance = int.Parse(Console.ReadLine());
                    volunteer = volunteer with { FullName = name, Phone = $"02-{phone}", Email = email, Password = encriptedPassword, FullAdress = adress, Latitude = latitude, Longitude = longtitude, MaxDistance = maxDistance };
                    s_dal.Volunteer?.Update(volunteer);
                }
                else
                {
                    Console.WriteLine("ERROR! Id's volunteer isn't existing");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Delete a Volunteer By Id
        private static void DeleteVolunteerById()
        {
            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                s_dal.Volunteer?.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Delete All the Volunteers
        private static void DeleteAllVolunteers()
        {
            try
            {
                s_dal.Volunteer?.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        // Repeat similar methods for CallMenu, AssignmentMenu, and ConfigMenu with CRUD operations

        private static void CallMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Call Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Add New Call");
                Console.WriteLine("2. Display Call by ID");
                Console.WriteLine("3. Display All Calls");
                Console.WriteLine("4. Update Call");
                Console.WriteLine("5. Delete Call by ID");
                Console.WriteLine("6. Delete All Calls");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        AddCall();
                        break;
                    case "2":
                        DisplayCallById();
                        break;
                    case "3":
                        DisplayAllCalls();
                        break;
                    case "4":
                        UpdateCall();
                        break;
                    case "5":
                        DeleteCallById();
                        break;
                    case "6":
                        DeleteAllCalls();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }

        }
        //add a nw call
        private static void AddCall()
        {
            try
            {
                // Collect call data
                Console.Write("Enter Call ID: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Enter Call Type (e.g., pastry, emergency, etc.): ");
                string callTypeStr = Console.ReadLine();
                if (!Enum.TryParse(callTypeStr, out DO.Enums.CallType callType))
                {
                    Console.WriteLine("Invalid call type. Setting to default (pastry).");
                    callType = DO.Enums.CallType.pastry; // Default to "pastry" if the input is invalid.
                }
                Console.Write("Enter Call Description: ");
                string description = Console.ReadLine();
                Console.Write("Enter Full Address: ");
                string fullAdress = Console.ReadLine();
                Console.Write("Enter Latitude: ");
                double latitude = double.Parse(Console.ReadLine());
                Console.Write("Enter Longitude: ");
                double longitude = double.Parse(Console.ReadLine());
                Console.Write("Enter Opening Call Time (yyyy-MM-dd HH:mm:ss): ");
                DateTime openingCallTime = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter Max Time to End (yyyy-MM-dd HH:mm:ss): ");
                DateTime maxTimeToEnd = DateTime.Parse(Console.ReadLine());

                // Create a new call object
                var call = new Call(id, callType, description, fullAdress, latitude, longitude, openingCallTime, maxTimeToEnd);
                s_dal.Call?.Create(call);
                Console.WriteLine("Call added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display a Call By Id
        private static void DisplayCallById()
        {
            try
            {
                Console.Write("Enter Call ID: ");
                int id = int.Parse(Console.ReadLine());
                var call = s_dal.Call?.Read(a => a.Id == id);
                if (call != null)
                {
                    Console.WriteLine(call);
                }
                else
                {
                    Console.WriteLine("Call not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display All the Calls
        private static void DisplayAllCalls()
        {
            try
            {
                var calls = s_dal.Call?.ReadAll();
                foreach (var call in calls)
                {
                    Console.WriteLine(call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Update a Call
        private static void UpdateCall()
        {
            try
            {
                Console.Write("Enter Call ID to update: ");
                int id = int.Parse(Console.ReadLine());
                var call = s_dal.Call?.Read(a => a.Id == id);
                if (call != null)
                {
                    Console.Write("Enter new Call Type (e.g., pastry, emergency, etc.): ");
                    string callTypeStr = Console.ReadLine();
                    if (!Enum.TryParse(callTypeStr, out DO.Enums.CallType callType))
                    {
                        Console.WriteLine("Invalid call type. Keeping the existing type.");
                        callType = call.CallType; // Keep the existing type if the input is invalid.
                    }
                    Console.Write("Enter new Call Description: ");
                    string description = Console.ReadLine();
                    Console.Write("Enter new Full Address: ");
                    string fullAdress = Console.ReadLine();
                    Console.Write("Enter new Latitude: ");
                    double latitude = double.Parse(Console.ReadLine());
                    Console.Write("Enter new Longitude: ");
                    double longitude = double.Parse(Console.ReadLine());
                    Console.Write("Enter new Opening Call Time (yyyy-MM-dd HH:mm:ss): ");
                    DateTime openingCallTime = DateTime.Parse(Console.ReadLine());
                    Console.Write("Enter new Max Time to End (yyyy-MM-dd HH:mm:ss): ");
                    DateTime maxTimeToEnd = DateTime.Parse(Console.ReadLine());

                    // Update call object
                    var updatedCall = call with
                    {
                        CallType = callType,
                        Description = description,
                        FullAdress = fullAdress,
                        Latitude = latitude,
                        Longitude = longitude,
                        OpeningCallTime = openingCallTime,
                        MaxTimeToEnd = maxTimeToEnd
                    };
                    s_dal.Call?.Update(updatedCall);
                    Console.WriteLine("Call updated successfully!");
                }
                else
                {
                    Console.WriteLine("Call not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        // Delete a Call By Id
        private static void DeleteCallById()
        {
            try
            {
                Console.Write("Enter Call ID to delete: ");
                int id = int.Parse(Console.ReadLine());
                s_dal.Call?.Delete(id);
                Console.WriteLine("Call deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Delete All the Calls
        private static void DeleteAllCalls()
        {
            try
            {
                s_dal.Call?.DeleteAll();
                Console.WriteLine("All calls deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Assignment Menu
        private static void AssignmentMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Assignment Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Add New Assignment");
                Console.WriteLine("2. Display Assignment by ID");
                Console.WriteLine("3. Display All Assignments");
                Console.WriteLine("4. Update Assignment");
                Console.WriteLine("5. Delete Assignment by ID");
                Console.WriteLine("6. Delete All Assignments");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        AddAssignment();
                        break;
                    case "2":
                        DisplayAssignmentById();
                        break;
                    case "3":
                        DisplayAllAssignments();
                        break;
                    case "4":
                        UpdateAssignment();
                        break;
                    case "5":
                        DeleteAssignmentById();
                        break;
                    case "6":
                        DeleteAllAssignments();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        //Add a new Assignment
        private static void AddAssignment()
        {
            try
            {
                Console.Write("Enter Assignment ID: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Enter Call ID: ");
                int callId = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer ID: ");
                int volunteerId = int.Parse(Console.ReadLine());
                Console.Write("Enter Entry Time for Treatment (yyyy-mm-dd hh:mm): ");
                DateTime entryTime = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter Actual Treatment End Time (yyyy-mm-dd hh:mm): ");
                DateTime endTime = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter Type of Treatment Termination (e.g., endTermCancelation): ");
                DO.Enums.TypeOfTreatmentTerm terminationType = (DO.Enums.TypeOfTreatmentTerm)Enum.Parse(typeof(DO.Enums.TypeOfTreatmentTerm), Console.ReadLine());

                var assignment = new Assignment(id, callId, volunteerId, entryTime, endTime, terminationType);
                s_dal.Assignment?.Create(assignment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display an Assignment By Id
        private static void DisplayAssignmentById()
        {
            try
            {
                Console.Write("Enter Assignment ID: ");
                int id = int.Parse(Console.ReadLine());
                var assignment = s_dal.Assignment?.Read(a => a.Id == id);
                Console.WriteLine(assignment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Display All the Assignments
        private static void DisplayAllAssignments()
        {
            try
            {
                var assignments = s_dal.Assignment?.ReadAll();
                foreach (var assignment in assignments)
                {
                    Console.WriteLine(assignment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Update an Assignment
        private static void UpdateAssignment()
        {
            try
            {
                Console.Write("Enter Assignment ID: ");
                int id = int.Parse(Console.ReadLine());
                var assignment = s_dal.Assignment?.Read(a => a.Id == id);
                if (assignment != null)
                {
                    Console.Write("Enter Call ID: ");
                    int callId = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer ID: ");
                    int volunteerId = int.Parse(Console.ReadLine());
                    Console.Write("Enter Entry Time for Treatment (yyyy-mm-dd hh:mm): ");
                    DateTime entryTime = DateTime.Parse(Console.ReadLine());
                    Console.Write("Enter Actual Treatment End Time (yyyy-mm-dd hh:mm): ");
                    DateTime endTime = DateTime.Parse(Console.ReadLine());
                    Console.Write("Enter Type of Treatment Termination (e.g., endTermCancelation): ");
                    DO.Enums.TypeOfTreatmentTerm terminationType = (DO.Enums.TypeOfTreatmentTerm)Enum.Parse(typeof(DO.Enums.TypeOfTreatmentTerm), Console.ReadLine());

                    assignment = assignment with { CallId = callId, VolunteerId = volunteerId, EntryTimeForTreatment = entryTime, ActualTreatmentEndTime = endTime, TypeOfTreatmentTermination = terminationType };
                    s_dal.Assignment?.Update(assignment);
                }
                else
                {
                    Console.WriteLine("ERROR! Assignment with the given ID doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Delete an Assignment By Id
        private static void DeleteAssignmentById()
        {
            try
            {
                Console.Write("Enter Assignment ID: ");
                int id = int.Parse(Console.ReadLine());
                s_dal.Assignment?.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Delete All the Assignments
        private static void DeleteAllAssignments()
        {
            try
            {
                s_dal.Assignment?.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Config Menu
        private static void ConfigMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Config Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Set Clock");
                Console.WriteLine("2. Set Risk Range");
                Console.WriteLine("3. Set Next Assignment ID");
                Console.WriteLine("4. Set Next Call ID");
                Console.WriteLine("5. Reset Config");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        SetClock();
                        break;
                    case "2":
                        SetRiskRange();
                        break;
                    case "3":
                        SetNextAssignmentId();
                        break;
                    case "4":
                        SetNextCallId();
                        break;
                    case "5":
                        ResetConfig();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        //Set the Clock
        private static void SetClock()
        {
            try
            {
                Console.Write("Enter Clock (yyyy-mm-dd hh:mm): ");
                DateTime clock = DateTime.Parse(Console.ReadLine());
                s_dal.Config.Clock = clock;
                Console.WriteLine("Clock set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Set the Risk Range
        private static void SetRiskRange()
        {
            try
            {
                Console.Write("Enter Risk Range (hours): ");
                TimeSpan riskRange = TimeSpan.FromHours(double.Parse(Console.ReadLine()));
                s_dal.Config.RiskRange = riskRange;
                Console.WriteLine("Risk Range set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Set the Next Assignment Id
        private static void SetNextAssignmentId()
        {
            try
            {
                Console.Write("Enter Next Assignment ID: ");
                int nextAssignmentId = int.Parse(Console.ReadLine());
                s_dal.Config.NextAssignmentId = nextAssignmentId;
                Console.WriteLine("Next Assignment ID set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Set teh Next Call Id
        private static void SetNextCallId()
        {
            try
            {
                Console.Write("Enter Next Call ID: ");
                int nextCallId = int.Parse(Console.ReadLine());
                s_dal.Config.NextCallId = nextCallId;
                Console.WriteLine("Next Call ID set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        //Reset the Config
        private static void ResetConfig()
        {
            try
            {
                s_dal.Config.Reset();
                Console.WriteLine("Config reset successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Show All the Data
        private static void ShowAllData()
        {
            try
            {
                DisplayAllVolunteers();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
        //Reset the Data
        private static void ResetData()
        {
            try
            {
                s_dal.Volunteer?.DeleteAll();
                s_dal.Call?.DeleteAll();
                s_dal.Assignment?.DeleteAll();
                s_dal.Config?.Reset();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
    }
}