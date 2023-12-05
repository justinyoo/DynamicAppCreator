using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class ColumnBinding
    {
        public long Server { get; set; }
        public long Database { get; set; }
        public string Table { get; set; }
        public string ValueColumn { get; set; }
        public string DisplayColumns { get; set; }
        public string Filter { get; set; }
    }
}
