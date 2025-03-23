using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

internal class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteer_xml = "volunteers.xml";
    internal const string s_assignment_xml = "assignments.xml";
    internal const string s_call_xml = "calls.xml";
    //...
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static TimeSpan RiskRange
    {
       get => XMLTools.GetConfigTimeVal(s_data_config_xml, "RiskRange");
       set => XMLTools.SetConfigTimeVal(s_data_config_xml, "RiskRange", value);
    }
    internal static void Reset()
    {
        NextCallId = 1000;
        NextAssignmentId = 1000;
        RiskRange = TimeSpan.Zero;
        Clock = DateTime.Now;
    }
    public static void SetNextAssignmentId(int value) {
        NextAssignmentId = value;
    }
    public static void SetNextCallId(int value)
    {
        NextCallId = value;
    }
}
