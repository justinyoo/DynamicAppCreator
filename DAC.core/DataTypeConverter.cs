using DAC.core.models;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAC.core
{
    public class DataTypeConverter
    {
        internal SqlDataType sqlDataType;
        private string name = string.Empty;

        public DataTypeConverter(SqlDataType sqlDataType)
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

        public DataTypeConverter() { }


        public static string GetSqlName(SqlDataType sqldt)
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
                SqlDataType.Xml => "xml",
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


        public static DataType ToDataType(Columns column)
        {

            DataType dt;
            switch (column.DataType)
            {
                case SqlDataType.BigInt: dt = DataType.BigInt; break;
                case SqlDataType.Binary: dt = DataType.Binary(int.Parse(column.Length)); break;
                case SqlDataType.Bit: dt = DataType.Bit; break;
                case SqlDataType.Char: dt = DataType.Char(int.Parse(column.Length)); break;
                case SqlDataType.DateTime: dt = DataType.DateTime; break;
                case SqlDataType.Decimal: dt = DataType.Decimal(0, 0); break;
                case SqlDataType.Numeric: dt = DataType.Numeric(0, 0); break;
                case SqlDataType.Float: dt = DataType.Float; break;
                case SqlDataType.Geography: dt = DataType.Geography; break;
                case SqlDataType.Geometry: dt = DataType.Geometry; break;
                case SqlDataType.Image: dt = DataType.Image; break;
                case SqlDataType.Int: dt = DataType.Int; break;
                case SqlDataType.Money: dt = DataType.Money; break;
                case SqlDataType.NChar: dt = DataType.NChar(int.Parse(column.Length)); break;
                case SqlDataType.NText: dt = DataType.NText; break;
                case SqlDataType.NVarChar: dt = column.Length.ToLower() != "max" ? DataType.NVarChar(int.Parse(column.Length)) : DataType.NVarCharMax; break;
                case SqlDataType.NVarCharMax: dt = DataType.NVarCharMax; break;
                case SqlDataType.Real: dt = DataType.Real; break;
                case SqlDataType.SmallDateTime: dt = DataType.SmallDateTime; break;
                case SqlDataType.SmallInt: dt = DataType.SmallInt; break;
                case SqlDataType.SmallMoney: dt = DataType.SmallMoney; break;
                case SqlDataType.Text: dt = DataType.Text; break;
                case SqlDataType.Timestamp: dt = DataType.Timestamp; break;
                case SqlDataType.TinyInt: dt = DataType.TinyInt; break;
                case SqlDataType.UniqueIdentifier: dt = DataType.UniqueIdentifier; break;
                case SqlDataType.UserDefinedDataType: dt = DataType.UserDefinedDataType(column.Length); break;
                case SqlDataType.UserDefinedTableType: dt = DataType.UserDefinedTableType(column.Length); break;
                case SqlDataType.UserDefinedType: dt = DataType.UserDefinedType(column.Length); break;
                case SqlDataType.VarBinary: dt = DataType.VarBinary(int.Parse(column.Length)); break;
                case SqlDataType.HierarchyId: dt = DataType.HierarchyId; break;
                case SqlDataType.VarBinaryMax: dt = DataType.VarBinaryMax; break;
                case SqlDataType.VarChar: dt = column.Length.ToLower() != "max" ? DataType.VarChar(int.Parse(column.Length)) : DataType.VarCharMax; break;
                case SqlDataType.VarCharMax: dt = DataType.VarCharMax; break;
                case SqlDataType.Variant: dt = DataType.Variant; break;
                case SqlDataType.Xml: dt = DataType.Xml(column.Length); break;
                case SqlDataType.SysName: dt = DataType.SysName; break;
                case SqlDataType.Date: dt = DataType.Date; break;
                case SqlDataType.Time: dt = DataType.DateTime; break;
                case SqlDataType.DateTimeOffset: dt = DataType.DateTimeOffset(int.Parse(column.Length)); break;
                case SqlDataType.DateTime2: dt = DataType.DateTime2(int.Parse(column.Length)); break;
                default:
                    dt = DataType.NVarCharMax; break;
            };
            return dt;
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
