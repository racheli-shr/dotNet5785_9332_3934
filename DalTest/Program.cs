namespace DalTest
//{
//    
//}
{
    using System;
    using Dal;
    using DalApi;
    using DO;
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        //private static IConfig? s_dalConfig = new ConfigImplementation();
        private static IConfig? s_dalConfig = (IConfig?)new ConfigImplementation();

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
                        Initialization.Do(s_dalAssignment, s_dalVolunteer, s_dalCall, s_dalConfig);
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
        private static void updatePasssword()
        {

            Console.WriteLine("enter the id of the volunteer to update his password");
            int id = int.Parse(Console.ReadLine());
            while (s_dalVolunteer.Read(id) == null)
            {
                Console.WriteLine("Id is not existing please r-enter the id of the volunteer to update his password");
                id = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
            string password = Console.ReadLine();
            while (!s_dalVolunteer.checkPassword(password))
            {
                Console.WriteLine("Sorry! Password is incorrect or not strong enough please enter again");
                Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                password = Console.ReadLine();
            }
            string encriptedPassword = s_dalVolunteer.EncryptPassword(password);
            s_dalVolunteer.updatePassword(id, encriptedPassword);
            Console.WriteLine("password updated successfully!");
        }
        
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
                string password = s_dalVolunteer.GenerateStrongPassword();//הגרלת סיסמא ראשונית
                Console.WriteLine($"Your password is: {password} you could change it at the volunteer menu.");
                string encriptedPassword = s_dalVolunteer.EncryptPassword(password);
                Console.Write("Enter Volunteer Adress: ");
                string adress = Console.ReadLine();
                Console.Write("Enter Volunteer latitude a number between -90 to 90: ");
                int latitude = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer longtitude a number between -180 to 180: ");
                int longtitude = int.Parse(Console.ReadLine());
                Console.Write("Enter Volunteer maxDistance in km: ");
                int maxDistance = int.Parse(Console.ReadLine());
                // יצירת אובייקט חדש והוספתו דרך הממשק
                var volunteer = new Volunteer(id, name, $"02-{phone}", email, encriptedPassword, adress, latitude, longtitude, Role.volunteer, true, maxDistance, DistanceType.airDistance);
                s_dalVolunteer?.Create(volunteer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void DisplayVolunteerById()
        {
            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                var volunteer = s_dalVolunteer?.Read(id);
                Console.WriteLine(volunteer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void DisplayAllVolunteers()
        {
            try
            {
                var volunteers = s_dalVolunteer?.ReadAll();
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

        private static void UpdateVolunteer()
        {
            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                var volunteer = s_dalVolunteer?.Read(id);
                if (volunteer != null)
                {
                    Console.Write("Enter Volunteer Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter House Phone Number: ");
                    int phone = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                    string password = Console.ReadLine();
                    while (!s_dalVolunteer.checkPassword(password))
                    {
                        Console.WriteLine("Sorry! Password is incorrect or not strong enough please enter again");
                        Console.Write("Enter a strong password of 8 characters with at least 1 digits ,one lowercase Letter, one uppercase lette:");
                        password = Console.ReadLine();
                    }
                    string encriptedPassword = s_dalVolunteer.EncryptPassword(password);
                    Console.Write("Enter Volunteer Adress: ");
                    string adress = Console.ReadLine();
                    Console.Write("Enter Volunteer latitude a number between -90 to 90: ");
                    int latitude = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer longtitude a number between -180 to 180: ");
                    int longtitude = int.Parse(Console.ReadLine());
                    Console.Write("Enter Volunteer maxDistance in km: ");
                    int maxDistance = int.Parse(Console.ReadLine());
                    volunteer = volunteer with { FullName = name, Phone = $"02-{phone}", Email = email, Password = encriptedPassword, FullAdress = adress, Latitude = latitude, Longitude = longtitude, MaxDistance = maxDistance };
                    s_dalVolunteer?.Update(volunteer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void DeleteVolunteerById()
        {
            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());
                s_dalVolunteer?.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void DeleteAllVolunteers()
        {
            try
            {
                s_dalVolunteer?.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        // Repeat similar methods for CallMenu, AssignmentMenu, and ConfigMenu with CRUD operations

        private static void CallMenu()
        {
            // Implement CallMenu similar to VolunteerMenu
        }

        private static void AssignmentMenu()
        {
            // Implement AssignmentMenu similar to VolunteerMenu
        }

        private static void ConfigMenu()
        {
            // Implement ConfigMenu similar to VolunteerMenu
        }

        private static void ShowAllData()
        {
            try
            {
                DisplayAllVolunteers();
                // Call similar methods to display all data for Calls, Assignments, and Configs
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void ResetData()
        {
            try
            {
                s_dalVolunteer?.DeleteAll();
                s_dalCall?.DeleteAll();
                s_dalAssignment?.DeleteAll();
                s_dalConfig?.Reset();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
    }
}