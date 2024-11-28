using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;


public interface IDal
{
    ICall call { get; }
    IAssignment assignment { get; }
    IVolunteer volunteer { get; }
    IConfig config { get; }
    void ResetDB();

}
