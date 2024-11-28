using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
using DO;

sealed public class DalList : IDal
{
    public ICall call { get; } = new CallImplementation();
    public IAssignment assignment { get; } = new AssignmentImplementation();

    public IVolunteer volunteer { get; } = new VolunteerImplementation();

    public IConfig config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        call.DeleteAll();
        assignment.DeleteAll();
        volunteer.DeleteAll();
        Config.Reset();
    }
}
