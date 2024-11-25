//namespace Dal;
//using DalApi;
//using DO;
//using System.Collections.Generic;
//public class CallImplementation : ICall
//{
//    public int Create(Call item)
//    {
//        Call c = item;
//        int newId = Config.NextCallId;
//        c.Id = newId;
//        DataSource.Calls.Add(c);
//        return newId;
//    }

//    public void Delete(int id)
//    {
//        Call c = Read(id);
//        if (c != null)
//        {
//            DataSource.Calls.Remove(c);
//        }
//        throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
//    }

//    public void DeleteAll()
//    {
//        for (int i = 0; i < DataSource.Calls.Count; i++)
//            DataSource.Calls.Remove(DataSource.Calls[i]);
//    }

//    public Call? Read(int id)
//    {
//        Call c = DataSource.Calls.Find(call => call.Id == id);
//        if (c != null) { return c; }
//        return null;
//    }

//    public List<Call> ReadAll()
//    {
//        return new List<Call>(DataSource.Calls);

//    }

//    public void Update(Call item)
//    {
//        Call c = DataSource.Calls.Find(call => call.Id == item.Id);
//        if (c != null) { throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים"); }
//        else
//        {
//            Delete(c.Id);
//            DataSource.Calls.Add(item);
//        }
//    }

//}
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public int Create(Call item)
    {
        // יצירת אובייקט חדש עם ID חדש
        int newId = Config.NextCallId;
        Call c = item with { Id = newId }; // יצירת אובייקט חדש עם ה-ID החדש
        DataSource.Calls.Add(c);
        return newId;
    }

    public void Delete(int id)
    {
        Call? c = Read(id);
        if (c != null)
        {
            DataSource.Calls.Remove(c);
        }
        else
        {
            throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        // חיפוש אובייקט Call לפי ID
        return DataSource.Calls.Find(call => call.Id == id);
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        Call? c = DataSource.Calls.Find(call => call.Id == item.Id);
        if (c != null)
        {
            // עדכון האובייקט על ידי מחיקת האובייקט הישן והוספת האובייקט החדש
            DataSource.Calls.Remove(c);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
        }
    }
}

