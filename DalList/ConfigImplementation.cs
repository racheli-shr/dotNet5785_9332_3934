

namespace Dal;

public class ConfigImplementation: IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public int NextAssignmentId
    {
        get => Config.NextAssignmentId;
        set => Config.NextAssignmentId = value;
    }
    public int NextCallId
    {
        get => Config.NextCallId;
        set => Config.NextCallId = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
}
