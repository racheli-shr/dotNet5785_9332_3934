using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    internal class CallInList
    {

        public int Id { get; set; } // Unique identifier for the assignment, not displayed in the UI
        public int CallId { get; set; } // Unique identifier for the call
        public CallType CallType { get; set; } // Enum representing the type of the call
        public DateTime OpeningTime { get; set; } // Time the call was opened
        public TimeSpan? RemainingTime { get; set; } // Time remaining until the maximum time for the call expires
        public string? LastVolunteerName { get; set; } // Name of the last volunteer who handled the call
        public TimeSpan? HandlingDuration { get; set; } // Duration of the call handling; relevant only for completed calls
        public CallStatus Status { get; set; } // Enum representing the current status of the call
        public int AssignmentCount { get; set; } // Total number of assignments related to this call

    }
}
