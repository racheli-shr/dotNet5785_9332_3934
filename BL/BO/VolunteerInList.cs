

using BL.Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; } // Unique identifier of the volunteer (from Volunteer.DO)
    public string FullName { get; set; } // Full name of the volunteer (first and last name)
    public bool IsActive { get; set; } // Indicates if the volunteer is currently active
    public int TotalCallsHandled { get; set; } // Total number of calls handled by the volunteer
    public int TotalCallsCanceled { get; set; } // Total number of calls canceled by the volunteer
    public int TotalExpiredCalls { get; set; } // Total number of calls that expired for the volunteer
    public int? CurrentCallId { get; set; } // ID of the call currently being handled by the volunteer, if any
    public BO.Enums.CallType CallType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
