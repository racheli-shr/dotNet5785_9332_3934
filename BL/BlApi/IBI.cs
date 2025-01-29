using BLApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi;

public interface IBl
{
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IAdmin Admin { get; }
}

