using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Enums;
namespace BlApi;


// Interface for Admin management
public interface IAdmin
{
    // Method to request the current system clock
    DateTime GetClock();

    // Method to advance the system clock by a specified time unit
    void ForwardClock(TimeUnit unit);

    // Method to request the current risk time span
    TimeSpan GetRiskTimeSpan();

    // Method to set the risk time span
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);

    // Method to reset the database (clear all data and configurations)
    void ResetDB();

    // Method to initialize the database (reset and populate with initial data)
    void InitializeDB();

    // Method to get the maximum range value
    TimeSpan GetMaxRange();

    // Method to set the maximum range value
    void SetMaxRange(TimeSpan maxRange);
}


