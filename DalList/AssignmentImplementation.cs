
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // יצירת אובייקט חדש עם ID חדש
        int newId = Config.NextAssignmentId;
        Assignment a = item with { Id = newId }; // יצירת אובייקט חדש עם ה-ID החדש
        DataSource.Assignments.Add(a);
        
    }

    public void Delete(int id)
    {
        Assignment? a = Read(id);
        if (a != null)
        {
            DataSource.Assignments.Remove(a);
        }
        else
        {
            throw new Exception("אובייקט מסוג Assignment עם ID כזה לא קיים");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        // חיפוש אובייקט Assignment לפי ID
        return DataSource.Assignments.Find(assignment => assignment.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? a = DataSource.Assignments.Find(assignment => assignment.Id == item.Id);
        if (a != null)
        {
            // עדכון האובייקט על ידי יצירת אובייקט חדש עם הערכים החדשים
            DataSource.Assignments.Remove(a);
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new Exception("אובייקט מסוג Assignment עם ID כזה לא קיים");
        }
    }
}
