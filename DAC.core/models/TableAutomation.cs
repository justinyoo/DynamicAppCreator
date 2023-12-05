using DAC.core.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class TableAutomation
    {
        public TriggerTypesEnum RunOn { get; set; }
        public string AutomationSource { get; set; }
    }
}
