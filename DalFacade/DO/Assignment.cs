using System;
using DO;
namespace DO;

public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTimeForTreatment,
    DateTime? ActualTreatmentEndTime,
    DO.Enums.AssignmentStatus AssignmentStatus
)
{
    // בנאי ברירת מחדל
    public Assignment() : this(0, 0, 0, DateTime.MinValue, null, DO.Enums.AssignmentStatus.NONE) { }
    public override string ToString()
    {
       
        return $@"
        Volunteer Details:
        -------------------
        ID: {Id}
        Call ID: {CallId}
        Volunteer ID: {VolunteerId}
        Entry Time For Treatment: {EntryTimeForTreatment}
        Actual Treatment End Time : {ActualTreatmentEndTime}
        Assignment Status: {AssignmentStatus}
        ";
    }
}
