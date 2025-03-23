using BL.Helpers;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BO.Enums;
namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; } // Unique running number for the call (from Call.DO)
    public CallType CallType { get; set; } // Type of the call (ENUM, from Call.DO)
    public string ?Address { get; set; } // Full address of the call (from Call.DO)
    public DateTime OpeningTime { get; set; } // Time when the call was opened (from Call.DO)
    public DateTime EntryTimeToHandle { get; set; } // Time when the call entered handling (from Assignment.DO)
    public DateTime? ActualFinishTime { get; set; } // Actual finish time of the call (nullable, from Assignment.DO)
    public TypeOfTreatmentTerm? FinishType { get; set; } // Type of finish for the call (nullable ENUM, from Assignment.DO)
    public override string ToString() => this.ToStringProperty();

}
