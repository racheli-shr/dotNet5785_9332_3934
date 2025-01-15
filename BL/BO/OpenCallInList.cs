using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    internal class OpenCallInList
    {

        public int Id { get; init; } // Unique running number for the call (from Call.DO)
        public CallType CallType { get; set; } // Type of the call (ENUM, from Call.DO)
        public string? Description { get; set; } // Textual description of the call (nullable, from Call.DO)
        public string Address { get; set; } // Full address of the call (from Call.DO)
        public DateTime OpeningTime { get; set; } // Time when the call was opened (from Call.DO)
        public DateTime? MaxFinishTime { get; set; } // Maximum time to finish the call (nullable, from Call.DO)
        public double DistanceFromVolunteer { get; set; } // Distance of the call from the volunteer
    }
}
