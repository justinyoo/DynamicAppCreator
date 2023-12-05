using DAC.core.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.kernel.models
{
    public class AppLogger
    {
        [Key] public Guid Id { get; set; }
        public string Scope { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Data { get; set; }

        public string Owner { get; set; }
        public LogTypesEnum Type { get; set; }

    }
}
