
using System.Runtime.CompilerServices;

namespace Dal;

internal static class DataSource
{
    internal static List<DO.Volunteer> Volunteers {

        [MethodImpl(MethodImplOptions.Synchronized)]
        get; } = new();
    internal static List<DO.Assignment> Assignments {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new();
    internal static List<DO.Call> Calls {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get; } = new();
}
