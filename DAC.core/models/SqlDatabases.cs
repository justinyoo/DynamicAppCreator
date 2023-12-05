using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class SqlDatabases
    {
        [Key]
        public long Id { get; set; }
        public long ServerID { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }
}
