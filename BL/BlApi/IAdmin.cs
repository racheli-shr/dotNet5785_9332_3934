using static BO.Enums;
namespace BlApi;


// Interface for Admin management
public interface IAdmin
{
    // Method to request the current system clock
    public DateTime GetClock();


    // Method to advance the system clock by a specified time unit
    public void ForwardClock(TimeUnit unit);

    // Method to request the current risk time span
    public TimeSpan GetRiskTimeSpan();

    // Method to set the risk time span
    public void SetRiskTimeSpan(TimeSpan riskTimeSpan);

    // Method to reset the database (clear all data and configurations)
    public void ResetDB();

    // Method to initialize the database (reset and populate with initial data)
    public void InitializeDB();

    // Method to get the maximum range value
    // בקשת הטווח המקסימלי
    public TimeSpan GetMaxRange();

    // Method to set the maximum range value
    public void SetMaxRange(TimeSpan maxRange);
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
    #region stage 7
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7

    #endregion
}


