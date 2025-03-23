
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using static DO.Exceptions;

internal class CallImplementation : ICall
{
    public Call? Read(int id)
    {
        Call? c = DataSource.Calls.FirstOrDefault(x => x.Id == id); // Fetch call by ID
        if (c == null)
            throw new DalDoesNotExistException($"No call found with ID {id}"); // Error if not found
        else return c;
    }

    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call c = item with { Id = newId }; 
        DataSource.Calls.Add(c);
        
    }

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

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(Func<Call, bool> filter)
    {
        
        return DataSource.Calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter != null
            ? from item in DataSource.Calls
              where filter(item)
              select item
            : from item in DataSource.Calls
              select item;
    }

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

