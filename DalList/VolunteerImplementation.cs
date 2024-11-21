

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if(Read(Volunteer.id))
        {
            throw new Exception("אובייקט מסוג T עם ID כזה כבר קיים");
        }
        DataSource.Volunteers.Add(item);    
    }

    public void Delete(int id)
    {
        Volunteer v = Read(id);
        if (v!=null)
        {
            DataSource.Volunteers.Remove(v);
        }
        throw new Exception("אובייקט מסוג Volunteer עם ID כזה לא קיים");
    }

    public void DeleteAll()
    {
        for (int i = 0; i < DataSource.Volunteers.Count; i++)
            DataSource.Volunteers.Remove(DataSource.Volunteers[i]);
    }

    public Volunteer? Read(int id)
    {
        Volunteer v= DataSource.Volunteers.Find(volunteer=>volunteer.Id==id);
        if(v!=null) { return v; }
        return null;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);

    }

    public void Update(Volunteer item)
    {
        Volunteer v = DataSource.Volunteers.Find(volunteer => volunteer.Id == item.Id);
        if (v!=null) { throw new Exception("אובייקט מסוג Volunteer עם ID כזה לא קיים"); }
        else
        {
            Delete(v.Id);
            DataSource.Volunteers.Add(item);
        }
    }
}
