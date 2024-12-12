

namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement s)
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
            Longitude= (double?)s.Element("Longtitude") ?? 0.0,
            Role= s.ToEnumNullable < Role >("Role")?? Role.volunteer,
            MaxDistance=(double?)s.Element("MaxDistance") ?? 0.0,
            DistanceType= s.ToEnumNullable<DistanceType>("Role") ?? DistanceType.airDistance
        };
    }



    public bool checkPassword(string password)
    {
        throw new NotImplementedException();
    }

    public void Create(Volunteer item)
    {
        throw new NotImplementedException();
    }

    public string DecryptPassword(string password)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        if (Volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteer_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteer_xml);
    }

    public string EncryptPassword(string password)
    {
        throw new NotImplementedException();
    }

    public string GenerateStrongPassword()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml).Elements().Select(s =>
        getVolunteer(s)).FirstOrDefault(filter);
    }


    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteer_xml);
        if (Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteer_xml);
    }

    public void updatePassword(int id, string password)
    {
        throw new NotImplementedException();
    }
}
