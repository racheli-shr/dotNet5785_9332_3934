

using System;

namespace DO;

public record Assignment
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime EntryTimeForTreatment { get; set; }
    public DateTime ActualTreatmentEndTime { get; set; }
    public DateTime TypeOfTreatmentTermination { get; set; }

    public Assignment()
    {
        Id = 0;  // ערך ברירת מחדל
        CallId = 0;  // ערך ברירת מחדל
        VolunteerId = 0;  // ערך ברירת מחדל
        EntryTimeForTreatment = DateTime.Now;  // זמן הנכנס לטיפול הוא הזמן הנוכחי
        ActualTreatmentEndTime = DateTime.Now.AddHours(1);  // זמן סיום טיפול משוער (למשל, שעה אחרי התחלת הטיפול)
        TypeOfTreatmentTermination = DateTime.Now.AddHours(2);  // זמן סיום טיפול משוער (למשל, שעתיים אחרי התחלת הטיפול)
    }
}
