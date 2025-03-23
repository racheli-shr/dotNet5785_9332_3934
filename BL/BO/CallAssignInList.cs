using BL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Enums;
namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; set; } // ID of the volunteer; nullable if assignment was artificially created
    public string? VolunteerName { get; set; } // Name of the volunteer; nullable if no volunteer is assigned
    public DateTime EntryTime { get; set; } // Time the volunteer started handling the call
    public DateTime? CompletionTime { get; set; } // Actual time the call handling was completed; nullable if not finished
    public AssignmentStatus? EndType { get; set; } // Type of assignment ending (ENUM)
    public override string ToString() => this.ToStringProperty();
}
