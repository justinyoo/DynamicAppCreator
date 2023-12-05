using DAC.core.models;
using System.ComponentModel.DataAnnotations;

namespace DynamicAppCreator.ModuleManagement.models
{
    public class Module
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ModuleSettings Settings { get; set; }
    }

    public class ModuleSettings
    {

        public long TableID { get; set; }
        public List<Columns> Columns { get; set; }



    }

    public class ModuleProperties
    {
        public bool AllowCreate { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool ShowInMenu { get; set; }
        public string MenuTitle { get; set; }
    }
}
