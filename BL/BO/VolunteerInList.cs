using BL.Helpers;
using Helpers;
using static BO.Enums;

namespace BO;

/// <summary>
/// Represents a volunteer in a list with basic information.
/// </summary>
public class VolunteerInList
{
    /// <summary>
    /// Gets or sets the unique identifier of the volunteer.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the full name of the volunteer.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the availability status of the volunteer.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the total number of calls treated by the volunteer.
    /// </summary>
    public int? SumTreatedCalls { get; set; }

    /// <summary>
    /// Gets or sets the total number of calls canceled by the volunteer.
    /// </summary>
    public int? SumCallsSelfCancelled { get; set; }

    /// <summary>
    /// Gets or sets the total number of calls that expired.
    /// </summary>
    public int? SumExpiredCalls { get; set; }

    /// <summary>
    /// Gets or sets the ID of the current call the volunteer is handling.
    /// </summary>
    public double? CallId { get; set; }

    /// <summary>
    /// Gets or sets the type of hosting service associated with the volunteer's current call.
    /// </summary>
    public CallType CallType { get; set; }

    /// <summary>
    /// Returns a string representation of the volunteer's properties.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
