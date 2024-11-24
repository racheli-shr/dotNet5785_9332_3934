

namespace DalApi;

public interface IConfig
{
    DateTime Clock { get; set; }
    int NextCallId { get; set; }
   int NextAssignmentId { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();
}
