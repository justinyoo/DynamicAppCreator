using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.kernel.models
{
    public class Permissions
    {
        [Key]
        public long Id { get; set; }
        [StringLength(100)]
        public string UserOrGroup { get; set; }
        [StringLength(100)]
        public string SectionName { get; set; }
        [StringLength(100)]
        public string SectionKey { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
        public bool Edit { get; set; }
    }
}
