using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
using DO;

sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }

    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
