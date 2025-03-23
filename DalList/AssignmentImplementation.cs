
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using static DO.Exceptions;

internal class AssignmentImplementation : IAssignment
{
    public Assignment? Read(int id)
    {
        Assignment? a = DataSource.Assignments.FirstOrDefault(a => a.Id == id); // Fetch assignment by ID
        if (a == null)
            throw new DalDoesNotExistException($"No assignment found with ID {id}"); // Error if not found
        else return a;
    }
    public void Create(Assignment item)
    {
       
        int newId = Config.NextAssignmentId;
        Assignment a = item with { Id = newId }; 
        DataSource.Assignments.Add(a);
        
    }

    public void Delete(int id)
    {
        Assignment? a = Read(a => a.Id == id);
        if (a != null)
        {
            DataSource.Assignments.Remove(a);
        }
        else
        {
            throw new DO.Exceptions.DalDoesNotExistException("Assignment ids doesn't exsisting");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter != null
            ? from item in DataSource.Assignments
              where filter(item)
              select item
            : from item in DataSource.Assignments
              select item;
    }

    public void Update(Assignment item)
    {
        Assignment? a = DataSource.Assignments.Find(assignment => assignment.Id == item.Id);
        if (a != null)
        {
            DataSource.Assignments.Remove(a);
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new DO.Exceptions.DalDoesNotExistException("Assignment ids doesn't exsisting");
        }
    }
}
