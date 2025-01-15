using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//using Helpers;
namespace BO;
public class Student
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string? password { get; set; }
    public string? address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude{ get; set; }
    public Role? role{ get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public DistanceType DistanceType { get; set; }
    public int numberOfCalls { get; set; }
    public int numberOfCanceledCalls { get; set; }
    public int numberOfexpiredCalls { get; set; }
    CallInProgress callInProgress { get; set; }
}
