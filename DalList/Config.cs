using System.Runtime.CompilerServices;

namespace Dal
{
    internal static class Config
    {        // Starting ID value for Call entities.

        internal const int startCallId = 1;
        private static int nextCallId = startCallId;
        // NextCallId: Returns the next available Call ID and increments the counter.

        public static int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]

            get => nextCallId++;
            [MethodImpl(MethodImplOptions.Synchronized)]

            set => nextCallId = value;
        }

        internal const int StartAssignmentId = 1;
        private static int nextAssignmentId = StartAssignmentId;
        // NextAssignmentId: Returns the next available Assignment ID and increments the counter.

        public static int NextAssignmentId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]

            get => nextAssignmentId++;
            [MethodImpl(MethodImplOptions.Synchronized)]

            set => nextAssignmentId = value;
        }

        public static DateTime Clock { get; set; } = DateTime.Now;
        public static TimeSpan RiskRange { get; set; } = TimeSpan.FromDays(5);

        // Reset: Resets the Call and Assignment ID counters and the Clock and RiskRange values to their initial defaults.
        [MethodImpl(MethodImplOptions.Synchronized)]

        public static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = StartAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromDays(18);
        }
    }
}
