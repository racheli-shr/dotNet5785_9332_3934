

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public int Create(Assignment item)
    {
        Assignment a = item;
        int newId = Config.NextAssignmentId;
        a.Id = newId;
        DataSource.Assignments.Add(a);  
        return newId;

    }

    public void Delete(int id)
    {
        Assignment a = Read(id);
        if (a != null)
        {
            DataSource.Assignments.Remove(a);
        }
        throw new Exception("אובייקט מסוג T עם ID כזה לא קיים");
    }

    public void DeleteAll()
    {
        for(int i = 0; i < DataSource.Assignments.Count; i++)
            DataSource.Assignments.Remove(DataSource.Assignments[i]);
    }

    public Assignment? Read(int id)
    {
        Assignment a = DataSource.Assignments.Find(assignment => assignment.Id == id);
        if (a != null) { return a; }
        return null;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);

    }

    public void Update(Assignment item)
    {
        Assignment a = DataSource.Assignments.Find(assignment => assignment.Id == item.Id);
        if (a != null) { throw new Exception("אובייקט מסוג Volunteer עם ID כזה לא קיים"); }
        else
        {
            Delete(a.Id);
            DataSource.Assignments.Add(item);
        }
    }
}
