namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
public class CallImplementation : ICall
{
    public int Create(Call item)
    {
        Call c = item;
        int newId = Config.NextCallId;
        c.Id = newId;
        DataSource.Calls.Add(c);
        return newId;
    }

    public void Delete(int id)
    {
        Call c = Read(id);
        if (c != null)
        {
            DataSource.Calls.Remove(c);
        }
        throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
    }

    public void DeleteAll()
    {
        for (int i = 0; i < DataSource.Calls.Count; i++)
            DataSource.Calls.Remove(DataSource.Calls[i]);
    }

    public Call? Read(int id)
    {
        Call c = DataSource.Calls.Find(call => call.Id == id);
        if (c != null) { return c; }
        return null;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);

    }

    public void Update(Call item)
    {
        Call c = DataSource.Calls.Find(call => call.Id == item.Id);
        if (c != null) { throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים"); }
        else
        {
            Delete(c.Id);
            DataSource.Calls.Add(item);
        }
    }

}


