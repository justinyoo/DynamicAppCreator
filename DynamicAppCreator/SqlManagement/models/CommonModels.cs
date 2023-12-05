using DAC.core.enums;
using DAC.core.models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DynamicAppCreator.SqlManagement.models
{
    public class CreateServerInputModel
    {
        [StringLength(100), Required] public string Name { get; set; }
        [AllowNull] public string Description { get; set; }
        [StringLength(100), Required] public string Username { get; set; }
        [StringLength(100), Required] public string Password { get; set; }
        [StringLength(255), Required] public string Server { get; set; }
        [StringLength(255), Required] public string DefaultDb { get; set; }
    }

    public class UpdateServerInputModel
    {
        [Key] public long Id { get; set; }
        [StringLength(100), Required] public string Name { get; set; }
        [AllowNull] public string Description { get; set; }
        [StringLength(100), Required] public string Username { get; set; }
        [StringLength(100), Required] public string Password { get; set; }
        [StringLength(255), Required] public string Server { get; set; }
        [StringLength(255), Required] public string DefaultDb { get; set; }
    }


    public class CreateDatabaseInputModel
    {
        public long Server { get; set; }
        [StringLength(100)] public string Name { get; set; }
        public string Description { get; set; }
        public DatabaseTypesEnum Type { get; set; }

    }


    public class UpdateDatabaseInputModel
    {
        [Key] public long Id { get; set; }
        public long Server { get; set; }
        [StringLength(100)][RegularExpression("([a-zA-Z]{1,25})([0-9_]{0,25})", ErrorMessage = "The database name accepts only alphanumeric characters, cannot start with a number or special character.")] public string Name { get; set; }
        public string Description { get; set; }
        public DatabaseTypesEnum Type { get; set; }

    }


    public class CreateTableInputModel
    {
        public long Database { get; set; }
        [StringLength(255)] public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string Schema { get; set; }
        public List<TableAutomation> TableAutomations { get; set; }
        public List<Columns> Columns { get; set; }
    }

    public class CreateExistingTableInputModel
    {
        public long Database { get; set; }
        [StringLength(255)] public string Name { get; set; }
        public string Description { get; set; }

    }

    public class UpdateTableInputModel : CreateTableInputModel
    {
        public long Id { get; set; }
    }

    public class CreateColumnInputModel
    {
        public Columns Column { get; set; }
        public long Table { get; set; }
    }
}
