
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class CallImplementation : ICall
{
    static Call getCall(XElement s)
    {
        return new DO.Call()
        {
            //Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            //FullName = (string?)s.Element("FullName") ?? "",
            //IsActive = (bool?)s.Element("IsActive") ?? false,
            //Phone = (string?)s.Element("Phone") ?? "",
            //Email = (string?)s.Element("Email") ?? "",
            //Password = (string?)s.Element("Password"),
            //FullAdress = (string?)s.Element("FullAdress"),
            //Latitude = (double?)s.Element("Latitude") ?? 0.0,
            //Longitude = (double?)s.Element("Longtitude") ?? 0.0,
            //Role = s.ToEnumNullable<Role>("Role") ?? Role.volunteer,
            //MaxDistance = (double?)s.Element("MaxDistance") ?? 0.0,
            //DistanceType = s.ToEnumNullable<DistanceType>("Role") ?? DistanceType.airDistance
        };
    }


    public void Create(Call item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_call_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_call_xml);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_call_xml);
    }
}
