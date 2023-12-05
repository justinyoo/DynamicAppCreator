using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    public class SqlServers
    {
        [Key] public long Id { get; set; }
        [StringLength(100)] public string Name { get; set; }
        [StringLength(255)] public string Description { get; set; }
        [StringLength(100)] public string Username { get; set; }
        [StringLength(100)] public string Password { get; set; }
        [StringLength(255)] public string Server { get; set; }
        [StringLength(255)] public string DefaultDb { get; set; }

    }
}
