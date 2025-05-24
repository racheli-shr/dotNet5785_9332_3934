namespace Dal
{
    internal static class Config
    {        // Starting ID value for Call entities.

        internal const int startCallId = 1;
        private static int nextCallId = startCallId;
        // NextCallId: Returns the next available Call ID and increments the counter.

        public static int NextCallId
        {
            get => nextCallId++;
            set => nextCallId = value;
        }

        internal const int StartAssignmentId = 1;
        private static int nextAssignmentId = StartAssignmentId;
        // NextAssignmentId: Returns the next available Assignment ID and increments the counter.

        public static int NextAssignmentId
        {
            get => nextAssignmentId++;
            set => nextAssignmentId = value;
        }

        public static DateTime Clock { get; set; } = DateTime.Now;
        public static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

        // Reset: Resets the Call and Assignment ID counters and the Clock and RiskRange values to their initial defaults.

        public static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = StartAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.Zero;
        }
    }
}
