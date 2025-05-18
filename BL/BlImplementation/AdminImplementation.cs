namespace BlImplementation;
using BlApi;
using DalApi;
using System;
using Helpers;
using static BO.Enums;

internal class AdminImplementation : IAdmin
{
    // Private read-only field for accessing DAL
    private readonly IDal _dal = DalApi.Factory.Get;

    // Variable for managing risk time
    private TimeSpan _riskTimeSpan = TimeSpan.Zero;

    // Variable for managing the maximum range
    private TimeSpan _maxRange;

    // Retrieve the current time
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    // Advance the clock by a specified time unit
    public void ForwardClock(TimeUnit unit)
    {
        switch (unit)
        {
            case TimeUnit.MINUTE:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case TimeUnit.HOUR:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case TimeUnit.DAY:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case TimeUnit.YEAR:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            case TimeUnit.MONTH:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported time unit.");
        }
    }

    // Retrieve the current risk time span
    public TimeSpan GetRiskTimeSpan()
    {
        return _riskTimeSpan;
    }

    // Set the risk time span
    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        _riskTimeSpan = riskTimeSpan;
    }
    public void InitializeDB()
    {
        AdminManager.InitializeDB();
    }
    public void ResetDB()
    {
        AdminManager.ResetDB();
    }

    // Reset the database
    //public void ResetDB()
    //{
    //    AdminManager.re(); // Call the reset method in DAL
    //    AdminManager.UpdateClock(AdminManager.Now); // Update the clock to the current time
    //    Console.WriteLine("Reset successfully");

    //}

    //// Initialize the database
    //public void InitializeDB()
    //{
    //    AdminManager.ResetDB(); // Call the reset method in DAL
    //    AdminManager.InitializeDB(); // Initialize the database
    //    AdminManager.UpdateClock(AdminManager.Now); // Update the clock to the current time
    //}

    // Retrieve the maximum range
    public TimeSpan GetMaxRange()
    {
        return AdminManager.MaxRange; // Use IDal.Config
    }

    // Set the maximum range
    public void SetMaxRange(TimeSpan maxRange)
    {
        if (maxRange <= TimeSpan.Zero)
        {
            throw new ArgumentException("Max range must be a positive value.");
        }
        _maxRange = maxRange; // Update local range variable
        AdminManager.MaxRange = maxRange; // Update value in DAL
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

}
