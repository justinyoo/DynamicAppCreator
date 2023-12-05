using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.kernel.models
{
    public class PermissionSubSettings
    {
        [Key]
        public long Id { get; set; }
        public ulong Permission { get; set; }
        [StringLength(100)]
        public string Scope { get; set; }
        [StringLength(100)]
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Allowed { get; set; }
    }
}
