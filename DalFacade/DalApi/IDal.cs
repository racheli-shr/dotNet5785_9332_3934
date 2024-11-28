using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;


public interface IDal
{
    ICall Call { get; }
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    IConfig Config { get; }
    void ResetDB();

}
