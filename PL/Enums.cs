using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    public class CallTypeCollection : IEnumerable
    {

        static readonly IEnumerable<BO.Enums.CallType> s_enums =
 (Enum.GetValues(typeof(BO.Enums.CallType)) as IEnumerable<BO.Enums.CallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();


    }

    public class VolunteerSortFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.VolunteerSortField> s_enums =
    (Enum.GetValues(typeof(BO.Enums.VolunteerSortField)) as IEnumerable<BO.Enums.VolunteerSortField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class VolunteerRoleCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.Role> s_enums =
    (Enum.GetValues(typeof(BO.Enums.Role)) as IEnumerable<BO.Enums.Role>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    public class distanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.Enums.DistanceType)) as IEnumerable<BO.Enums.DistanceType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class callTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.CallType> s_enums =
    (Enum.GetValues(typeof(BO.Enums.CallType)) as IEnumerable<BO.Enums.CallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}

