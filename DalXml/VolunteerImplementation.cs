namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using static DO.Exceptions;

internal class VolunteerImplementation : IVolunteer
{
  //  convert from element to volunteer object
    static Volunteer GetVolunteer(XElement s)
    {
        return new DO.Volunteer()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)s.Element("FullName") ?? "",
            IsActive = (bool?)s.Element("IsActive") ?? false,
            Phone = (string?)s.Element("Phone")??"",
            Email = (string?)s.Element("Email")??"",
            Password = (string?)s.Element("Password"),
            FullAdress = (string?)s.Element("FullAdress"),
            Latitude=(double?)s.Element("Latitude")?? 0.0,
            longtitude= (double?)s.Element("Longtitude") ?? 0.0,
            Role= s.ToEnumNullable <DO.Enums.Role >("Role")?? DO.Enums.Role.volunteer,
            MaxDistance=(double?)s.Element("MaxDistance") ?? 0.0,
            DistanceType= s.ToEnumNullable<DO.Enums.DistanceType>("Role") ?? DO.Enums.DistanceType.airDistance
        };
    }


    //verify if the password is legal and srtong
    public bool checkPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length != 8)
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    public void Create(Volunteer item)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        if (volunteers.Exists(v => v.Id == item.Id))
            throw new DO.Exceptions.DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists.");

        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteer_xml);
        
    }
    //Decrypt the Password
    public string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            throw new ArgumentException("Encrypted password cannot be null or empty.");

        StringBuilder decryptedPassword = new StringBuilder();

        foreach (char ch in encryptedPassword)
            decryptedPassword.Append((char)(ch - 2));

        return decryptedPassword.ToString();
    }

    public void Delete(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        if (Volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteer_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteer_xml);
    }
    //Encrypt the Password
    public string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.");

        StringBuilder encryptedPassword = new StringBuilder();

        foreach (char ch in password)
            encryptedPassword.Append((char)(ch + 2));

        return encryptedPassword.ToString();
    }
    //genarate first password for a new person
    public string GenerateStrongPassword()
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "@$!%*?&";

        Random random = new Random();

        char upper = upperChars[random.Next(upperChars.Length)];
        char lower = lowerChars[random.Next(lowerChars.Length)];
        char digit = digits[random.Next(digits.Length)];
        char special = specialChars[random.Next(specialChars.Length)];

        string allChars = upperChars + lowerChars + digits + specialChars;
        string randomChars = new string(Enumerable.Range(0, 4)
            .Select(_ => allChars[random.Next(allChars.Length)]).ToArray());

        string password = upper.ToString() + lower + digit + special + randomChars;
        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        Volunteer? volunteer = volunteers.FirstOrDefault(filter);
        Console.WriteLine(volunteers.FirstOrDefault(filter));
        if (volunteer == null)
        {
            throw new DalDoesNotExistException("No matching volunteer found.");
        }

        return volunteer;

    }

    public Volunteer Read(int id)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        Volunteer? volunteer = volunteers.FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
        {
            throw new DalDoesNotExistException($"No volunteer found with ID {id}.");
        }

        return volunteer;
    }




        public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        if (Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteer_xml);
    }
    //change and update a new password
    public void updatePassword(int id, string password)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        var volunteer = volunteers.FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
            throw new DO.Exceptions.DalDoesNotExistException("אובייקט מסוג Volunteer עם ID כזה לא קיים");

        volunteer = volunteer with { Password = EncryptPassword(password) };
        volunteers.RemoveAll(v => v.Id == id);
        volunteers.Add(volunteer);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteer_xml);
    }
}
