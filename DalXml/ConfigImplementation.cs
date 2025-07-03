using DalApi;
using DO;
using System;
using System.Runtime.CompilerServices;

namespace Dal
{
    internal class ConfigImplementation : IConfig
    {
        public DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => Config.Clock;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => Config.Clock = value;
        }
        public TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => Config.RiskRange;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => Config.RiskRange = value;
        }
        public int NextAssignmentId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => Config.NextAssignmentId;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => Config.SetNextAssignmentId(value);
        }

        public int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]

            get => Config.NextCallId;
            [MethodImpl(MethodImplOptions.Synchronized)]

            set => Config.SetNextCallId(value);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            Config.Reset();
        }
    }
}
