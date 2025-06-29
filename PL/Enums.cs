using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    // This class provides an enumerable collection of all values in the CallType enum.
    public class CallTypeCollection : IEnumerable
    {

        static readonly IEnumerable<BO.Enums.CallType> s_enums =
        (Enum.GetValues(typeof(BO.Enums.CallType)) as IEnumerable<BO.Enums.CallType>)!;

        // Returns an enumerator that iterates through all CallType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    // This class provides an enumerable collection of all values in the CallType enum.
    public class ClosedCallInListFieldsCollection : IEnumerable
    {

        static readonly IEnumerable<BO.Enums.ClosedCallInListFields> s_enums =
        (Enum.GetValues(typeof(BO.Enums.ClosedCallInListFields)) as IEnumerable<BO.Enums.ClosedCallInListFields>)!;

        // Returns an enumerator that iterates through all CallType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    // This class provides an enumerable collection of all values in the VolunteerSortField enum.
    public class VolunteerSortFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.VolunteerSortField> s_enums =
    (Enum.GetValues(typeof(BO.Enums.VolunteerSortField)) as IEnumerable<BO.Enums.VolunteerSortField>)!;

        // Returns an enumerator that iterates through all VolunteerSortField enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    // This class provides an enumerable collection of all values in the Role enum.
    public class VolunteerRoleCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.Role> s_enums =
    (Enum.GetValues(typeof(BO.Enums.Role)) as IEnumerable<BO.Enums.Role>)!;

        // Returns an enumerator that iterates through all Role enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    // This class provides an enumerable collection of all values in the DistanceType enum.
    public class distanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.Enums.DistanceType)) as IEnumerable<BO.Enums.DistanceType>)!;

        // Returns an enumerator that iterates through all DistanceType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    // This class (with lowercase name) also provides an enumerable collection of all CallType enum values.
    public class callTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.CallType> s_enums =
    (Enum.GetValues(typeof(BO.Enums.CallType)) as IEnumerable<BO.Enums.CallType>)!;

        // Returns an enumerator that iterates through all CallType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    // This class (with lowercase name) also provides an enumerable collection of all CallType enum values.
    public class EndOfTreatmentCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.TypeOfTreatmentTerm> s_enums =
    (Enum.GetValues(typeof(BO.Enums.TypeOfTreatmentTerm)) as IEnumerable<BO.Enums.TypeOfTreatmentTerm>)!;

        // Returns an enumerator that iterates through all CallType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class OpenCallsFieldsCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Enums.OpenCallInListFields> s_enums =
    (Enum.GetValues(typeof(BO.Enums.OpenCallInListFields)) as IEnumerable<BO.Enums.OpenCallInListFields>)!;

        // Returns an enumerator that iterates through all CallType enum values.
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

}


