using BL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Enums;
namespace BO;

public class CallInProgress
{
    public int Id { get; init; } // Unique identifier for the entity
    public int CallId { get; init; } // Unique identifier for the call
    public CallType CallType { get; set; } // Type of the call (ENUM)
    public string? Description { get; set; } // Textual description of the call
    public string Address { get; set; } // Full address of the call
    public DateTime OpeningTime { get; set; } // Time when the call was opened
    public DateTime? MaxFinishTime { get; set; } // Maximum time to complete the call
    public DateTime EntryTimeToHandle { get; set; } // Time the volunteer started handling the call
    public double? DistanceFromVolunteer { get; set; } // Distance from the volunteer to the call
    public CallStatus Status { get; set; } // Current status of the call (ENUM)
    public override string ToString() => this.ToStringProperty();

}
