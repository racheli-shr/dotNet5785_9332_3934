using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DalApi;
namespace Dal;

sealed internal class DalXml : IDal
{
    public static IDal Instance {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new DalXml();
    [MethodImpl(MethodImplOptions.Synchronized)]

    private DalXml()
    {
    
    }

    public ICall Call {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } =new CallImplementation();

    public IAssignment Assignment {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } =new AssignmentImplementation();

    public IVolunteer Volunteer {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } =new VolunteerImplementation();

    public IConfig Config {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } =new ConfigImplementation();
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll(); 
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
