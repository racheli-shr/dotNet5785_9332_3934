using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    internal class ClosedCallInList
    {
        public int Id { get; init; } // Unique running number for the call (from Call.DO)
        public CallType CallType { get; set; } // Type of the call (ENUM, from Call.DO)
        public string Address { get; set; } // Full address of the call (from Call.DO)
        public DateTime OpeningTime { get; set; } // Time when the call was opened (from Call.DO)
        public DateTime EntryTimeToHandle { get; set; } // Time when the call entered handling (from Assignment.DO)
        public DateTime? ActualFinishTime { get; set; } // Actual finish time of the call (nullable, from Assignment.DO)
        public FinishType? FinishType { get; set; } // Type of finish for the call (nullable ENUM, from Assignment.DO)

    }
}
