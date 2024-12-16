
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class CallImplementation : ICall
{
   // public Call() : this(0) { };
    static Call GetCall(XElement s)
    {
        return new DO.Call()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallType = s.ToEnumNullable<CallType>("CallType") ?? CallType.pastry,  // הגדרת enum עם סוג "CallType"
            Description= (string?)s.Element("Description") ?? "",
            FullAdress= (string?)s.Element("FullAdress") ?? "",
            Latitude = (double?)s.Element("Latitude") ?? 0.0,
            Longitude = (double?)s.Element("Longtitude") ?? 0.0,
            OpeningCallTime = s.ToDateTimeNullable("OpeningCallTime") ?? DateTime.Now,
            MaxTimeToEnd = s.ToDateTimeNullable("MaxTimeToEnd") ?? DateTime.Now,
        };
    }


    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.Any(c => c.Id == item.Id))
            throw new DalAlreadyExistsException($"Call with ID={item.Id} already exists");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
        ;
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
        return XMLTools.LoadListFromXMLElement(Config.s_call_xml).Elements().Select(s =>
        GetCall(s)).FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return filter != null ? calls.Where(filter) : calls;

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
