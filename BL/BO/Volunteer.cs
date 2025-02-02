
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Enums;

//using Helpers;
namespace BO;
public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude{ get; set; }
    public Role? Role{ get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public DistanceType DistanceType { get; set; }
    public int NumberOfCalls { get; set; }
    public int NumberOfCanceledCalls { get; set; }
    public int NumberOfexpiredCalls { get; set; }
    public BO.CallInProgress? CurrentCallInProgress { get; set; }
}
