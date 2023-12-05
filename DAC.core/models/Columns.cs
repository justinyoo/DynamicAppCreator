using DAC.core.enums;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DAC.core.models
{
    public class Columns
    {
        public string Name { get; set; } = "";
        public string Schema { get; set; } = "";
        public string Description { get; set; } = "";

        public string Length { get; set; } = "";
        public int Size { get; set; } = 50;
        public EncryptionTypesEnum Encryption { get; set; } = EncryptionTypesEnum.none;
        public int ColumnOrder { get; set; } = 0;
        public int Type { get; set; } = 0;
        public string? DefaultValueOrBinding { get; set; } = "";
        public ComputedColumnSpecification? ComputedColumnSpecification { get; set; } = new ComputedColumnSpecification();
        public SqlDataType DataType { get; set; } = SqlDataType.NVarChar;
        public DataOrderProperties DataOrder { get; set; } = new DataOrderProperties();
        public List<ColumnMaskValidation> Validations { get; set; } = new List<ColumnMaskValidation>();
        public ColumnBinding Binding { get; set; } = new ColumnBinding();
        //  public ColumnAutomations Automations { get; set; } = new ColumnAutomations();
        public List<ColumnValueAutomation> Automations { get; set; } = new List<ColumnValueAutomation>();
        public ColumnProperties Properties { get; set; } = new ColumnProperties();
    }
}
