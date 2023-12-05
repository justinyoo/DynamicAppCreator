using DAC.core.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class ColumnValueAutomation
    {
        public bool RunOnCreating { get; set; }
        public bool RunOnUpdating { get; set; }
        public ValueAutomationTypes ValueSource { get; set; }
        public bool Enable { get; set; }
        public string CustomValueOrClaim { get; set; }
    }
}
