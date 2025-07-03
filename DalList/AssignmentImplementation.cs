
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static DO.Exceptions;

internal class AssignmentImplementation : IAssignment
{
    // Retrieves an Assignment object by its unique ID.
    // Throws an exception if the assignment is not found.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(int id)
    {
        Assignment? a = DataSource.Assignments.FirstOrDefault(a => a.Id == id); // Fetch assignment by ID
        if (a == null)
            throw new DalDoesNotExistException($"No assignment found with ID {id}"); // Error if not found
        else return a;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Assignment item)
    {    // Creates a new Assignment entry with a unique ID and adds it to the data source.


        int newId = Config.NextAssignmentId;
        Assignment a = item with { Id = newId }; 
        DataSource.Assignments.Add(a);
        
    }
    // Deletes an Assignment entry by its ID.
    // Throws an exception if the assignment does not exist.
    [MethodImpl(MethodImplOptions.Synchronized)]

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
    // Deletes all Assignment entries from the data source.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    // Retrieves a single Assignment that matches a given filter condition.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
    // Retrieves all Assignments, optionally filtered by a given condition.
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter != null
            ? from item in DataSource.Assignments
              where filter(item)
              select item
            : from item in DataSource.Assignments
              select item;
    }
    // Updates an existing Assignment entry.
    // Replaces the old entry with the new one.
    // Throws an exception if the assignment does not exist.
    [MethodImpl(MethodImplOptions.Synchronized)]

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
