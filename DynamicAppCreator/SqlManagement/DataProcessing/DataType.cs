using System.Xml.XPath;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;

namespace DynamicAppCreator.SqlManagement.DataProcessing
{
    internal class DataType
    {
        internal SqlDataType sqlDataType;
        private string name = string.Empty;


        public DataType(SqlDataType sqlDataType)
        {
            switch (sqlDataType)
            {
                case SqlDataType.BigInt:
                case SqlDataType.Binary:
                case SqlDataType.Bit:
                case SqlDataType.Char:
                case SqlDataType.DateTime:
                case SqlDataType.Image:
                case SqlDataType.Int:
                case SqlDataType.Money:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.Real:
                case SqlDataType.SmallDateTime:
                case SqlDataType.SmallInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Text:
                case SqlDataType.Timestamp:
                case SqlDataType.TinyInt:
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                case SqlDataType.Variant:
                case SqlDataType.Xml:
                case SqlDataType.SysName:
                case SqlDataType.Date:
                case SqlDataType.HierarchyId:
                case SqlDataType.Geometry:
                case SqlDataType.Geography:
                    this.sqlDataType = sqlDataType;
                    name = GetSqlName(sqlDataType);
                    break;
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                    this.sqlDataType = sqlDataType;
                    name = GetSqlName(sqlDataType);
                    //NumericPrecision = 18;
                    //NumericScale = 0;
                    break;
                case SqlDataType.Float:
                    this.sqlDataType = sqlDataType;
                    name = GetSqlName(sqlDataType);
                    //NumericPrecision = 53;
                    break;
                case SqlDataType.Time:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.DateTime2:
                    this.sqlDataType = sqlDataType;
                    name = GetSqlName(sqlDataType);
                    //NumericScale = 7;
                    break;
                    // throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
            }
        }
        public DataType() { }

        public string GetSqlName(SqlDataType sqldt)
        {
            return sqldt switch
            {
                SqlDataType.BigInt => "bigint",
                SqlDataType.Binary => "binary",
                SqlDataType.Bit => "bit",
                SqlDataType.Char => "char",
                SqlDataType.DateTime => "datetime",
                SqlDataType.Decimal => "decimal",
                SqlDataType.Numeric => "numeric",
                SqlDataType.Float => "float",
                SqlDataType.Geography => "geography",
                SqlDataType.Geometry => "geometry",
                SqlDataType.Image => "image",
                SqlDataType.Int => "int",
                SqlDataType.Money => "money",
                SqlDataType.NChar => "nchar",
                SqlDataType.NText => "ntext",
                SqlDataType.NVarChar => "nvarchar",
                SqlDataType.NVarCharMax => "nvarchar",
                SqlDataType.Real => "real",
                SqlDataType.SmallDateTime => "smalldatetime",
                SqlDataType.SmallInt => "smallint",
                SqlDataType.SmallMoney => "smallmoney",
                SqlDataType.Text => "text",
                SqlDataType.Timestamp => "timestamp",
                SqlDataType.TinyInt => "tinyint",
                SqlDataType.UniqueIdentifier => "uniqueidentifier",
                SqlDataType.UserDefinedDataType => string.Empty,
                SqlDataType.UserDefinedTableType => string.Empty,
                SqlDataType.UserDefinedType => string.Empty,
                SqlDataType.VarBinary => "varbinary",
                SqlDataType.HierarchyId => "hierarchyid",
                SqlDataType.VarBinaryMax => "varbinary",
                SqlDataType.VarChar => "varchar",
                SqlDataType.VarCharMax => "varchar",
                SqlDataType.Variant => "sql_variant",
                SqlDataType.Xml => "",
                SqlDataType.SysName => "sysname",
                SqlDataType.Date => "date",
                SqlDataType.Time => "time",
                SqlDataType.DateTimeOffset => "datetimeoffset",
                SqlDataType.DateTime2 => "datetime2",
                _ => string.Empty,
            };
        }

        public static Type GetSystemType(SqlDataType sqldt)
        {
            return sqldt switch
            {
                SqlDataType.BigInt => typeof(long),
                SqlDataType.Binary => typeof(BinaryData),
                SqlDataType.Bit => typeof(bool),
                SqlDataType.Char => typeof(string),
                SqlDataType.DateTime => typeof(DateTime),
                SqlDataType.Decimal => typeof(decimal),
                SqlDataType.Numeric => typeof(int),
                SqlDataType.Float => typeof(float),
                SqlDataType.Geography => typeof(object),
                SqlDataType.Geometry => typeof(object),
                SqlDataType.Image => typeof(byte[]),
                SqlDataType.Int => typeof(int),
                SqlDataType.Money => typeof(float),
                SqlDataType.NChar => typeof(string),
                SqlDataType.NText => typeof(string),
                SqlDataType.NVarChar => typeof(string),
                SqlDataType.NVarCharMax => typeof(string),
                SqlDataType.Real => typeof(int),
                SqlDataType.SmallDateTime => typeof(DateTime),
                SqlDataType.SmallInt => typeof(Int16),
                SqlDataType.SmallMoney => typeof(float),
                SqlDataType.Text => typeof(string),
                SqlDataType.Timestamp => typeof(TimeSpan),
                SqlDataType.TinyInt => typeof(byte),
                SqlDataType.UniqueIdentifier => typeof(Guid),
                SqlDataType.UserDefinedDataType => typeof(object),
                SqlDataType.UserDefinedTableType => typeof(object),
                SqlDataType.UserDefinedType => typeof(object),
                SqlDataType.VarBinary => typeof(byte[]),
                SqlDataType.HierarchyId => typeof(object),
                SqlDataType.VarBinaryMax => typeof(byte[]),
                SqlDataType.VarChar => typeof(string),
                SqlDataType.VarCharMax => typeof(string),
                SqlDataType.Variant => typeof(object),
                SqlDataType.Xml => typeof(XmlDocument),
                SqlDataType.SysName => typeof(string),
                SqlDataType.Date => typeof(DateOnly),
                SqlDataType.Time => typeof(DateTime),
                SqlDataType.DateTimeOffset => typeof(string),
                SqlDataType.DateTime2 => typeof(DateTime),
                _ => typeof(object),
            };
        }


        public static Type GetSystemType(string sqldt)
        {
            return sqldt switch
            {
                "bigint" => GetSystemType(SqlDataType.BigInt),
                "binary" => GetSystemType(SqlDataType.Binary),
                "bit" => GetSystemType(SqlDataType.Bit),
                "char" => GetSystemType(SqlDataType.Char),
                "datetime" => GetSystemType(SqlDataType.DateTime),
                "decimal" => GetSystemType(SqlDataType.Decimal),
                "numeric" => GetSystemType(SqlDataType.Numeric),
                "float" => GetSystemType(SqlDataType.Float),
                "geography" => GetSystemType(SqlDataType.Geography),
                "geometry" => GetSystemType(SqlDataType.Geometry),
                "image" => GetSystemType(SqlDataType.Image),
                "int" => GetSystemType(SqlDataType.Int),
                "money" => GetSystemType(SqlDataType.Money),
                "nchar" => GetSystemType(SqlDataType.NChar),
                "ntext" => GetSystemType(SqlDataType.NText),
                "nvarchar" => GetSystemType(SqlDataType.NVarChar),
                "nvarcharmax" => GetSystemType(SqlDataType.NVarCharMax),
                "real" => GetSystemType(SqlDataType.Real),
                "smalldatetime" => GetSystemType(SqlDataType.SmallDateTime),
                "smallint" => GetSystemType(SqlDataType.SmallInt),
                "smallmoney" => GetSystemType(SqlDataType.SmallMoney),
                "text" => GetSystemType(SqlDataType.Text),
                "timestamp" => GetSystemType(SqlDataType.Timestamp),
                "tinyint" => GetSystemType(SqlDataType.TinyInt),
                "uniqueidentifier" => GetSystemType(SqlDataType.UniqueIdentifier),
                "userdefineddatatype" => GetSystemType(SqlDataType.UserDefinedDataType),
                "userdefinedtabletype" => GetSystemType(SqlDataType.UserDefinedTableType),
                "userdefinedtype" => GetSystemType(SqlDataType.UserDefinedType),
                "varbinary" => GetSystemType(SqlDataType.VarBinary),
                "hierarchyid" => GetSystemType(SqlDataType.HierarchyId),
                "varbinarymax" => GetSystemType(SqlDataType.VarBinaryMax),
                "varchar" => GetSystemType(SqlDataType.VarChar),
                "varcharmax" => GetSystemType(SqlDataType.VarCharMax),
                "variant" => GetSystemType(SqlDataType.Variant),
                "xml" => GetSystemType(SqlDataType.Xml),
                "sysname" => GetSystemType(SqlDataType.SysName),
                "date" => GetSystemType(SqlDataType.Date),
                "time" => GetSystemType(SqlDataType.Time),
                "datetimeoffset" => GetSystemType(SqlDataType.DateTimeOffset),
                "datetime2" => GetSystemType(SqlDataType.DateTime2),
                _ => GetSystemType(SqlDataType.NVarChar)
            };
        }
    }
}
