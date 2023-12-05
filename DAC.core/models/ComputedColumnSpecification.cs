using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{

    public class ComputedColumnSpecification
    {
        public bool isPersisted { get; set; } = false;
        public string Formula { get; set; } = "";
    }
}
