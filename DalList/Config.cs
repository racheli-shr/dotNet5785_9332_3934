
namespace Dal;

internal static class Config
{
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId
    {
        get => nextCallId++;
        set => nextCallId = value; // Add set accessor
    }

    internal const int StartAssignmentId = 1;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId
    {
        get => nextAssignmentId++;
        set => nextAssignmentId = value; // Add set accessor
    }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}

