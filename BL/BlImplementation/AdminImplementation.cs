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

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7
    public void StartSimulator(int interval)  //stage 7
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.Start(interval); //stage 7
        }catch(Exception ex) { throw new Exception(ex.Message); }

    }
    // Advance the clock by a specified time unit
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        _riskTimeSpan = riskTimeSpan;
    }
    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.InitializeDB();
    }
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.ResetDB();
    }
    public TimeSpan GetMaxRange()
    {
        return AdminManager.MaxRange; // Use IDal.Config
    }

    // Set the maximum range
    public void SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        if (maxRange <= TimeSpan.Zero)
        {
            throw new ArgumentException("Max range must be a positive value.");
        }
        _maxRange = maxRange; // Update local range variable
        AdminManager.MaxRange = maxRange; // Update value in DAL
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ClockUpdatedObservers += clockObserver;
    }
    public void RemoveClockObserver(Action clockObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ClockUpdatedObservers -= clockObserver;
    }
    public void AddConfigObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ConfigUpdatedObservers += configObserver;
    }
    public void RemoveConfigObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ConfigUpdatedObservers -= configObserver;
    }

    public void AddRiskObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ConfigUpdatedObservers += configObserver;
    }
    public void RemoveRiskObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ConfigUpdatedObservers -= configObserver;
    }
    #endregion Stage 5

}
