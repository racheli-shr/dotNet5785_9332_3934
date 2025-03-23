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
        return ClockManager.Now;
    }

    // Advance the clock by a specified time unit
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

    // Reset the database
    public void ResetDB()
    {
        _dal.ResetDB(); // Call the reset method in DAL
        ClockManager.UpdateClock(ClockManager.Now); // Update the clock to the current time
        Console.WriteLine("Reset successfully");

    }

    // Initialize the database
    public void InitializeDB()
    {
        _dal.ResetDB(); // Call the reset method in DAL
        DalTest.Initialization.Do(); // Initialize the database
        ClockManager.UpdateClock(ClockManager.Now); // Update the clock to the current time
    }

    // Retrieve the maximum range
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange; // Use IDal.Config
    }

    // Set the maximum range
    public void SetMaxRange(TimeSpan maxRange)
    {
        if (maxRange <= TimeSpan.Zero)
        {
            throw new ArgumentException("Max range must be a positive value.");
        }
        _maxRange = maxRange; // Update local range variable
        _dal.Config.RiskRange = maxRange; // Update value in DAL
    }
}
