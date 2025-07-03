using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

sealed internal class DalList : IDal
{
    public static IDal Instance {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new DalList();
    [MethodImpl(MethodImplOptions.Synchronized)]

    private DalList() { }

    public ICall Call {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get; 
    } = new CallImplementation();
    public IAssignment Assignment {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new AssignmentImplementation();

    public IVolunteer Volunteer {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
