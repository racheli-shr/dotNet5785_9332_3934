
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static DO.Exceptions;

internal class CallImplementation : ICall
{
    public Call Read(int id)
    {
        // Load the list of calls from XML
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);

        // Try to find the call by ID
        Call? call = calls.FirstOrDefault(c => c.Id == id);

        // If no call was found, throw an exception
        if (call == null)
        {
            throw new DalDoesNotExistException($"No call found with ID {id}");
        }

        return call;
    }

    // convert from element to call object
    static Call GetCall(XElement s)
    {
        return new DO.Call()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallType = s.ToEnumNullable<DO.Enums.CallType>("CallType") ?? DO.Enums.CallType.pastry,  // הגדרת enum עם סוג "CallType"
            Description = (string?)s.Element("Description") ?? "",
            FullAdress = (string?)s.Element("FullAdress") ?? "",
            Latitude = (double?)s.Element("Latitude") ?? 0.0,
            longtitude = (double?)s.Element("longtitude") ?? 0.0,
            OpeningCallTime = s.ToDateTimeNullable("OpeningCallTime") ?? DateTime.Now,
            MaxTimeToEnd = s.ToDateTimeNullable("MaxTimeToEnd") ?? DateTime.Now,
        };
    }


    public void Create(Call item)
    {
        // bring the next id from the data config
        int newId = XMLTools.GetAndIncreaseConfigIntVal(Config.s_data_config_xml, "NextCallId");

        //create a new object

        Call c = item with { Id = newId };

        // load all exsisting call and check that there are not 2 calls with the same ID
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.Any(existingCall => existingCall.Id == c.Id))
            throw new DO.Exceptions.DalAlreadyExistsException($"Call with ID={c.Id} already exists");

        //add a new call to the list
        calls.Add(c);

        // save at the XML file
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={id} does Not exist");
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
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_call_xml);
    }
}
