
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
   // public Assignment() : this(0) { };
    public void Create(Assignment item)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        throw new NotImplementedException();
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
