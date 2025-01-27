namespace BlApi;
using BlApi;
using BO;
using DalApi;
using DO;
public interface IVolunteer
{
    /// <summary>
    /// מתודת כניסה למערכת
    /// </summary>
    /// <param name="username">שם משתמש</param>
    /// <param name="password">סיסמה</param>
    /// <returns>מחרוזת תוצאה של ההתחברות</returns>
    Task<string> LoginAsync(string username, string password);

    /// <summary>
    /// מתודת בקשת רשימת מתנדבים
    /// </summary>
    /// <param name="isActive">מצב הפעילות של המתנדב (אופציונלי)</param>
    /// <param name="sortBy">שדה למיון (אופציונלי)</param>
    /// <returns>רשימה של מתנדבים</returns>
    internal Task<IEnumerable<VolunteerInList>> GetVolunteerListAsync(bool? isActive, VolunteerSortField? sortBy);

    /// <summary>
    /// מתודת בקשת פרטי מתנדב
    /// </summary>
    /// <param name="id">מזהה המתנדב</param>
    /// <returns>אובייקט עם פרטי המתנדב</returns>
    Task<Volunteer> GetVolunteerDetailsAsync(string id);

    /// <summary>
    /// מתודת עדכון פרטי מתנדב
    /// </summary>
    /// <param name="id">מזהה המתנדב</param>
    /// <param name="volunteer">אובייקט עם הפרטים המעודכנים</param>
    Task UpdateVolunteerAsync(string id, Volunteer volunteer);

    /// <summary>
    /// מתודת בקשת מחיקת מתנדב
    /// </summary>
    /// <param name="id">מזהה המתנדב למחיקה</param>
    Task DeleteVolunteerAsync(string id);

    /// <summary>
    /// מתודת הוספת מתנדב
    /// </summary>
    /// <param name="volunteer">אובייקט עם פרטי המתנדב החדש</param>
    Task AddVolunteerAsync(Volunteer volunteer);
}