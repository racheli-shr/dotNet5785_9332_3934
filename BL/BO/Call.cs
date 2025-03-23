using BO;
using static BO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Helpers;

namespace BO;

public class Call
{
    public int Id { get; init; } // Unique running number for the call (from Call.DO)
    public CallType CallType { get; set; } // Type of the call (ENUM, from Call.DO)
    public string? Description { get; set; } // Textual description of the call (nullable, from Call.DO)
    public string? Address { get; set; } // Full address of the call; validated in the logic layer
    public double Latitude { get; set; } // Latitude of the call's location; updated when address is set
    public double Longitude { get; set; } // Longitude of the call's location; updated when address is set
    public DateTime OpeningTime { get; init; } // Time the call was opened; set by the data layer
    public DateTime? MaxFinishTime { get; set; } // Maximum time to finish the call; must be validated in logic layer
    public CallStatus Status { get; set; } // Status of the call; computed based on logic conditions
    public List<CallAssignInList>? Assignments { get; set; } // List of assignments for this call; nullable if none exist
    public override string ToString() => this.ToStringProperty();

}
