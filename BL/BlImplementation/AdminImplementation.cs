namespace BlImplementation;
using BlApi;
using BO; // הנחה שהמחלקה TimeUnit מוגדרת כאן
using DalApi; // לשימוש ב- IDal
using System;
using Helpers;
using static BO.Enums;

internal class AdminImplementation : IAdmin
{
    // שדה פרטי לקריאה בלבד לגישה ל-DAL
    private readonly IDal _dal = DalApi.Factory.Get;

    // משתנה לניהול זמן סיכון
    private TimeSpan _riskTimeSpan = TimeSpan.Zero;

    // משתנה לניהול הטווח המקסימלי
    private TimeSpan _maxRange;

    // קבלת הזמן הנוכחי
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    // קידום השעון לפי יחידת זמן
    public void ForwardClock(TimeUnit unit)
    {
        switch (unit)
        {
            case TimeUnit.MINUTE:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                break;
            case TimeUnit.HOUR:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                break;
            case TimeUnit.DAY:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported time unit.");
        }
    }

    // בקשת זמן הסיכון הנוכחי
    public TimeSpan GetRiskTimeSpan()
    {
        return _riskTimeSpan;
    }

    // הגדרת זמן הסיכון
    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        _riskTimeSpan = riskTimeSpan;
    }

    // איפוס בסיס הנתונים
    public void ResetDB()
    {
        _dal.ResetDB(); // קריאה למתודת האיפוס ב-DAL
        ClockManager.UpdateClock(ClockManager.Now); // עדכון השעון לזמן הנוכחי
    }

    // אתחול בסיס הנתונים
    public void InitializeDB()
    {
        _dal.ResetDB(); // קריאה למתודת האיפוס
        DalTest.Initialization.Do(); // אתחול בסיס הנתונים
        ClockManager.UpdateClock(ClockManager.Now); // עדכון השעון לזמן הנוכחי
    }

    // בקשת הטווח המקסימלי
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange; // שימוש ב-IDal.Config
    }

    // הגדרת הטווח המקסימלי
    public void SetMaxRange(TimeSpan maxRange)
    {
        if (maxRange <= TimeSpan.Zero)
        {
            throw new ArgumentException("Max range must be a positive value.");
        }
        _maxRange = maxRange; // עדכון הטווח המקומי
        _dal.Config.RiskRange = maxRange; // עדכון הערך ב-DAL
    }
}
