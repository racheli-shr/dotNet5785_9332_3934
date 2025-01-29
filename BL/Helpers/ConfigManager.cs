

using DalApi;

namespace Helpers;

internal static class ConfigManager
{
    private static IDal config_dal = Factory.Get; //stage 4
    public static TimeSpan GetRiskTimeSpanFromDal()
    {
        return TimeSpan.Zero;
    }

}
