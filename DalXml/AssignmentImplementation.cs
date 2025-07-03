
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using static DO.Exceptions;

internal class AssignmentImplementation : IAssignment
{
    // convert from element to assignment object
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment Read(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml)
            .Elements()
            .FirstOrDefault(a => (int?)a.Element("Id") == id);

        if (assignmentElem is null)
            throw new DalDoesNotExistException($"No assignment found with ID {id}.");

        return GetAssignment(assignmentElem);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]

    static Assignment GetAssignment(XElement s)
    {
        var status = s.ToEnumNullable<DO.Enums.AssignmentStatus>("AssignmentStatus");
        return new DO.Assignment()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = s.ToIntNullable("CallId") ?? 0,
            VolunteerId= s.ToIntNullable("VolunteerId") ?? 0,
            EntryTimeForTreatment= s.ToDateTimeNullable("EntryTimeForTreatment") ?? DateTime.Now,
            ActualTreatmentEndTime=s.ToDateTimeNullable("ActualTreatmentEndTime") ?? DateTime.Now,
            AssignmentStatus= s.ToEnumNullable<DO.Enums.AssignmentStatus>("AssignmentStatus") ?? DO.Enums.AssignmentStatus.NONE,
        };
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Assignment item)
    {
        // bring the next id from the data config
        int newId = XMLTools.GetAndIncreaseConfigIntVal(Config.s_data_config_xml, "NextAssignmentId");

        //create a new object
        Assignment c = item with { Id = newId };

        // load all exsisting assigment and check that there are not 2 assigments with the same ID
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (assignments.Any(existingAssignment => existingAssignment.Id == c.Id))
            throw new DO.Exceptions.DalAlreadyExistsException($"Assignment with ID={c.Id} already exists");

        //add a new assigment to the list
        assignments.Add(c);

        // save at the XML file
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);

    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignment_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var assignments= XMLTools.LoadListFromXMLElement(Config.s_assignment_xml).Elements().Select(s =>
        GetAssignment(s)).FirstOrDefault(filter);
        return assignments;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        return filter != null ? assignments.Where(filter) : assignments;

    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DO.Exceptions.DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);
    }
}
