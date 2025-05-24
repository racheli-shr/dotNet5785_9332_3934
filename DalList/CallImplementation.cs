
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using static DO.Exceptions;

internal class CallImplementation : ICall
{
    // Retrieves a Call object by its unique ID.
    // Throws an exception if the call is not found.
    public Call? Read(int id)
    {
        Call? c = DataSource.Calls.FirstOrDefault(x => x.Id == id); // Fetch call by ID
        if (c == null)
            throw new DalDoesNotExistException($"No call found with ID {id}"); // Error if not found
        else return c;
    }
    // Creates a new Call entry with a unique ID and adds it to the data source.

    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call c = item with { Id = newId }; 
        DataSource.Calls.Add(c);
        
    }
    // Deletes a Call entry by its ID.
    // Throws an exception if the call does not exist.
    public void Delete(int id)
    {
        Call? c = Read(a => a.Id == id);
        if (c != null)
        {
            DataSource.Calls.Remove(c);
        }
        else
        {
            throw new DO.Exceptions.DalDoesNotExistException("Call ids doesn't exsisting");
        }
    }
    // Deletes all Call entries from the data source.

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    // Retrieves a single Call entry that matches a given filter condition.

    public Call? Read(Func<Call, bool> filter)
    {
        
        return DataSource.Calls.FirstOrDefault(filter);
    }
    // Retrieves all Call entries, optionally filtered by a given condition.

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter != null
            ? from item in DataSource.Calls
              where filter(item)
              select item
            : from item in DataSource.Calls
              select item;
    }
    // Updates an existing Call entry.
    // Replaces the old entry with the new one.
    // Throws an exception if the call does not exist.
    public void Update(Call item)
    {
        Call? c = DataSource.Calls.Find(call => call.Id == item.Id);
        if (c != null)
        {
           
            DataSource.Calls.Remove(c);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new DO.Exceptions.DalDoesNotExistException("Call ids doesn't exsisting");
        }
    }
}

