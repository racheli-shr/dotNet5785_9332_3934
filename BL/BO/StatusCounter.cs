using BL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class StatusCounter
    {
        public Enums.CallStatus status { get; set; }
        public int Count { get; set; } = 0;
        public override string ToString() => this.ToStringProperty();
    }
}
