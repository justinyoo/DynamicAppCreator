using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class ColumnAutomations
    {
        public List<ColumnValueAutomation> onUpdating { get; set; } = new List<ColumnValueAutomation>();
        public List<ColumnValueAutomation> onInserting { get; set; } = new List<ColumnValueAutomation>();
        public List<ColumnValueAutomation> onDeleting { get; set; } = new List<ColumnValueAutomation>();
        public List<ColumnValueAutomation> onFiltering { get; set; } = new List<ColumnValueAutomation>();
    }
}
