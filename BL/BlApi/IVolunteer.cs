using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using BO;
using DO;
namespace BlApi;

public interface IVolunteer
{

    // מתודת כניסה למערכת
    Task<string> LoginAsync(string username, string password);

    // מתודת בקשת רשימת מתנדבים
    internal Task<IEnumerable<VolunteerInList>> GetVolunteerListAsync(
        bool? isActive,
        VolunteerSortField? sortBy
    );

    // מתודת בקשת פרטי מתנדב
    Task<Volunteer> GetVolunteerDetailsAsync(string id);

    // מתודת עדכון פרטי מתנדב
    Task UpdateVolunteerAsync(string id, Volunteer volunteer);

    // מתודת בקשת מחיקת מתנדב
    Task DeleteVolunteerAsync(string id);

    // מתודת הוספת מתנדב
    Task AddVolunteerAsync(Volunteer volunteer);
}






