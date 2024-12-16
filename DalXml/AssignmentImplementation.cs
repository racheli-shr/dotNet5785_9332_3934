
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    // public Assignment() : this(0) { };GetAssignment

    static Assignment GetAssignment(XElement s)
    {
        return new DO.Assignment()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = s.ToIntNullable("CallId") ?? 0,
            VolunteerId= s.ToIntNullable("VolunteerId") ?? 0,
            EntryTimeForTreatment= s.ToDateTimeNullable("EntryTimeForTreatment") ?? DateTime.Now,
            ActualTreatmentEndTime=s.ToDateTimeNullable("ActualTreatmentEndTime") ?? DateTime.Now,
            TypeOfTreatmentTermination= s.ToEnumNullable<TypeOfTreatmentTerm>("TypeOfTreatmentTermination") ?? TypeOfTreatmentTerm.endTermCancelation,
        };
    }
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (assignments.Any(a => a.Id == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml);

    }

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignment_xml);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignment_xml).Elements().Select(s =>
        GetAssignment(s)).FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        return filter != null ? assignments.Where(filter) : assignments;

    }

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignment_xml);
    }
}
