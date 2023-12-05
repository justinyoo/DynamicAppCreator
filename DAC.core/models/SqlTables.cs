using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.models
{
    [Table("SqlTables")]
    public class SqlTables
    {
        [Key]
        public Int64 Id { get; set; }
        public long Database { get; set; }
        [StringLength(255)] public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string ColumnSettings { get; set; }
        public string AutomationSettings { get; set; }
        public string? Schema { get; set; }
        public bool Existing { get; set; }
        public int Version { get; set; }
        public List<TableAutomation> TableAutomations()
        {

            return AutomationSettings != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<TableAutomation>>(AutomationSettings) : new List<TableAutomation>();
        }
        public List<Columns> Columns()
        {
            return ColumnSettings != "" ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Columns>>(ColumnSettings) : new List<Columns>();
        }

        public string ToEntityName()
        {
            return $"{this.Name}_{this.Version}";
        }
    }
}
