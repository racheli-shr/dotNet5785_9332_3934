
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // יצירת אובייקט חדש עם ID חדש
        int newId = Config.NextCallId;
        Call c = item with { Id = newId }; // יצירת אובייקט חדש עם ה-ID החדש
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
            throw new Exception("אובייקט מסוג Call עם ID כזה לא קיים");
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(Func<Call, bool> filter)
    {
        // חיפוש אובייקט Assignment לפי הפילטר
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

