using System.ComponentModel.DataAnnotations;

namespace DAC.core.models
{
    public class Modules
    {
        [Key]
        public long Id { get; set; }
        public long Table { get; set; }

        [StringLength(100), Required]
        public string Name { get; set; } = "";

        [StringLength(100)]
        public string Description { get; set; } = "";

        public string Settings { get; set; } = "";

        public bool ShowInMenu { get; set; } = false;
        [StringLength(100), Required] public string MenuTitle { get; set; }

    }
}
