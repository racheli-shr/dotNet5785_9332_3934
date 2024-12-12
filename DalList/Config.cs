namespace Dal
{
    internal static class Config
    {
        internal const int startCallId = 1;
        private static int nextCallId = startCallId;
        public static int NextCallId
        {
            get => nextCallId++;
            set => nextCallId = value;
        }

        internal const int StartAssignmentId = 1;
        private static int nextAssignmentId = StartAssignmentId;
        public static int NextAssignmentId
        {
            get => nextAssignmentId++;
            set => nextAssignmentId = value;
        }

        public static DateTime Clock { get; set; } = DateTime.Now;
        public static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

        public static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = StartAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.Zero;
        }
    }
}
