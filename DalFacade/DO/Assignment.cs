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
    DO.Enums.TypeOfTreatmentTerm? TypeOfTreatmentTermination
)
{
    // בנאי ברירת מחדל
    public Assignment() : this(0, 0, 0, DateTime.MinValue, null, null) { }
}
