using DAC.core;
using DAC.core.models;

namespace DynamicAppCreator.SqlManagement
{
    public class CreateSqlTable : SqlTables
    {
        public string SqlScript { get; set; }
    }
    public class MigrationBuilder
    {
        private SqlTables model;
        public MigrationBuilder(CreateSqlTable model)
        {
            this.model = model;
        }

        private System.Text.StringBuilder SqlString = new System.Text.StringBuilder();
        private Dictionary<string, string> ColumnSql { get; set; } = new Dictionary<string, string>();
        private IEnumerable<Columns> oldColumns;
        public string CreateMigrationSql(IEnumerable<Columns> oldColumns)
        {
            this.oldColumns = oldColumns;
            SetSqlScript();
            TableExist();
            ColumnExist();
            GeneratePrimaryKeys();
            return SqlString.ToString();

        }
        private void TableExist()
        {
            SqlString.AppendLine(string.Format(" IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = '{0}')) ", model.Name));
            SqlString.AppendLine(" BEGIN ");
            SqlString.AppendLine(string.Format("    {0} ", CreateTableSql()));
            SqlString.AppendLine(" END ");

        }
        private void ColumnExist()
        {
            SqlString.AppendLine(" ELSE  BEGIN ");

            SqlString.AppendLine($"         IF EXISTS(SELECT *   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME='PK_{model.Name}')");
            SqlString.AppendLine($"         BEGIN ALTER TABLE {model.Name} DROP CONSTRAINT PK_{model.Name}; END");


            var IXColumns = "";
            foreach (var item in model.Columns().Where(t => t.Properties.isIndex))
            {
                if (IXColumns == "") { IXColumns = item.Name + (item.Properties.IndexDescending ? " DESC" : " ASC"); } else { IXColumns = $"{IXColumns},{item.Name} {(item.Properties.IndexDescending ? " DESC" : " ASC")}"; }
            }
            if (!string.IsNullOrEmpty(IXColumns))
            {
                SqlString.AppendLine($" IF EXISTS(SELECT * FROM sys.indexes WHERE name='IX_{model.Name}' AND object_id = OBJECT_ID('{model.Name}')) ");
                SqlString.AppendLine($"     BEGIN DROP INDEX [IX_{model.Name}] ON  [{model.Name}] WITH ( ONLINE = OFF ) END");
            }

            //removed columns
            foreach (var item in this.oldColumns.Where(t => !string.IsNullOrEmpty(t.DefaultValueOrBinding) && !model.Columns().Any(tx => tx.Name == t.Name)))
            {
                SqlString.AppendLine($" IF EXISTS(SELECT * FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{model.Name}' AND COLUMN_NAME = '{item.Name}') ");
                SqlString.AppendLine($"     BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} DROP COLUMN {item.Name}  ");
                SqlString.AppendLine($"     END ");
            }

            //removed columns
            foreach (var item in model.Columns().Where(t => this.oldColumns.Any(tx => tx.Length != t.Length && tx.DataType == t.DataType)))
            {
                if (string.IsNullOrEmpty(item.Length)) { item.Length = "50"; }
                SqlString.AppendLine($" IF EXISTS(SELECT * FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{model.Name}' AND COLUMN_NAME = '{item.Name}') ");
                SqlString.AppendLine($"     BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} ALTER COLUMN {item.Name} {item.DataType} ({item.Length}) ");
                SqlString.AppendLine($"     END ");
            }

            var counter = 0;

            //Clear pk constranits
            foreach (var item in model.Columns().Where(t => !string.IsNullOrEmpty(t.DefaultValueOrBinding)))
            {
                counter++;
                SqlString.AppendLine($" IF EXISTS(SELECT *   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME='DF_{model.Name}_{item.Name}')");
                SqlString.AppendLine($"     BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} DROP CONSTRAINT DF_{model.Name}_{item.Name};  ");
                SqlString.AppendLine($"     END ");

            }

            //Configure Columns (with datatypes)
            foreach (var item in model.Columns().Where(x => !isComputedSpecifiation(x)))
            {
                SqlString.AppendLine($" IF NOT EXISTS(SELECT * FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{model.Name}' AND COLUMN_NAME = '{item.Name}') ");
                SqlString.AppendLine($"     BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} ADD {ColumnSql[item.Name]}  ");
                SqlString.AppendLine($"     END ");



                if (!item.Properties.isIdentity)
                {
                    SqlString.AppendLine($"     ELSE BEGIN   ");
                    SqlString.AppendLine($"         ALTER TABLE {model.Name} ALTER COLUMN {ColumnSql[item.Name]} ");
                    SqlString.AppendLine($"     END ");
                }
                else
                {
                    SqlString.AppendLine($"     ELSE BEGIN   ");
                    SqlString.AppendLine($"         ALTER TABLE {model.Name} ALTER COLUMN {ColumnSql[item.Name].Replace("IDENTITY(1,1)", "")} ");
                    SqlString.AppendLine($"     END ");
                }


            }

            foreach (var item in model.Columns().Where(x => isComputedSpecifiation(x)))
            {
                SqlString.AppendLine($"     IF NOT EXISTS(SELECT * FROM   INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{model.Name}' AND COLUMN_NAME = '{item.Name}') ");
                SqlString.AppendLine($"     BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} ADD {ColumnSql[item.Name]}  ");
                SqlString.AppendLine($"     END ");
                SqlString.AppendLine($"     ELSE BEGIN ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} DROP COLUMN {item.Name}  ");
                SqlString.AppendLine($"         ALTER TABLE {model.Name} ADD {ColumnSql[item.Name]}  ");
                SqlString.AppendLine($"     END ");
            }

            SqlString.AppendLine(" END ");
            //


        }
        private string CreateTableSql()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($" CREATE TABLE [{model.Name}]");
            sb.AppendLine($" ({GetColumnSqlScript()}) ");
            return sb.ToString();
        }
        private void GeneratePrimaryKeys()
        {



            var IXColumns = "";
            foreach (var item in model.Columns().Where(t => t.Properties.isIndex))
            {
                if (IXColumns == "") { IXColumns = "[" + item.Name + "]" + " ASC"; } else { IXColumns = $"{IXColumns},[{item.Name}] ASC"; }
            }
            if (!string.IsNullOrEmpty(IXColumns))
            {

                SqlString.AppendLine($" CREATE CLUSTERED INDEX [IX_{model.Name}] ON [{model.Name}] ({IXColumns}); ");
            }

            var PkColumns = "";
            foreach (var item in model.Columns().Where(t => t.Properties.isPrimary && string.IsNullOrEmpty(t.DefaultValueOrBinding)))
            {
                if (PkColumns == "") { PkColumns = "[" + item.Name + "]"; } else { PkColumns = $"{PkColumns},[{item.Name}]"; }

            }
            if (!string.IsNullOrEmpty(PkColumns))
            {
                if (string.IsNullOrEmpty(IXColumns))
                {
                    SqlString.AppendLine($" ALTER TABLE [{model.Name}] ADD CONSTRAINT PK_{model.Name} PRIMARY KEY CLUSTERED({PkColumns});");
                }
                else
                {
                    SqlString.AppendLine($" ALTER TABLE [{model.Name}] ADD CONSTRAINT PK_{model.Name} PRIMARY KEY NONCLUSTERED({PkColumns});");
                }

            }

            var counter = 0;
            foreach (var item in model.Columns().Where(t => !isComputedSpecifiation(t) && !string.IsNullOrEmpty(t.DefaultValueOrBinding)))
            {
                counter++;
                SqlString.AppendLine($" ALTER TABLE [{model.Name}] ADD CONSTRAINT DF_{model.Name}_{item.Name} DEFAULT({item.DefaultValueOrBinding}) FOR [{item.Name}] ");
            }
        }
        private bool isComputedSpecifiation(Columns item)
        {
            if (item.ComputedColumnSpecification == null)
            {
                return false;
            }
            else if (string.IsNullOrEmpty(item.ComputedColumnSpecification.Formula))
            {
                return false;
            }

            return true;
        }
        private void SetSqlScript()
        {

            foreach (var item in model.Columns())
            {


                if (isComputedSpecifiation(item) && !item.Properties.isIdentity)
                {
                    ColumnSql.Add(item.Name, $"[{item.Name}] AS ({item.ComputedColumnSpecification.Formula}) ");
                    if (item.ComputedColumnSpecification.isPersisted)
                    {
                        ColumnSql[item.Name] = ColumnSql[item.Name] + $" PERSISTED";
                    }
                }

                if (!isComputedSpecifiation(item))
                {
                    ColumnSql.Add(item.Name, $"[{item.Name}] [{item.DataType}] ");

                    switch (DataTypeConverter.GetSqlName(item.DataType).ToLower())
                    {
                        case "numeric":
                        case "decimal":
                        case "float":
                        case "money":
                            if (string.IsNullOrEmpty(item.Length))
                            {
                                item.Length = "18,4";
                            }
                            else if (item.Length.Split(",").Length < 2)
                            {
                                item.Length = "18,4";
                            }
                            ColumnSql[item.Name] = ColumnSql[item.Name] + $" ({item.Length})";
                            break;
                        case "char":
                        case "nchar":
                        case "nvarchar":
                        case "varchar":
                        case "varbinary":
                            if (item.Length == "") { item.Length = "50"; }
                            ColumnSql[item.Name] = ColumnSql[item.Name] + $" ({item.Length})";
                            break;
                        case "datetime2":
                            if (!int.TryParse(item.Length, out int length))
                            {
                                item.Length = "7";
                            }
                            ColumnSql[item.Name] = ColumnSql[item.Name] + $" ({item.Length})";
                            break;
                        default:
                            ColumnSql[item.Name] = ColumnSql[item.Name] + " ";
                            break;
                    }



                    if (item.Properties.isIdentity)
                    {
                        switch (DataTypeConverter.GetSqlName(item.DataType).ToLower())
                        {
                            case "bigint":
                            case "int":
                            case "smallint":
                            case "tinyint":
                            case "decimal":
                            case "numeric":
                                ColumnSql[item.Name] = ColumnSql[item.Name] + $" IDENTITY(1,1)";
                                break;
                            default:
                                break;
                        }
                    }


                    if (item.Properties.AllowNull && !item.Properties.isPrimary)
                    {
                        ColumnSql[item.Name] = ColumnSql[item.Name] + $" NULL ";
                    }
                    else
                    {
                        ColumnSql[item.Name] = ColumnSql[item.Name] + $" NOT NULL ";
                    }

                }

                //}
            }
        }
        private string GetColumnSqlScript()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in model.Columns().Where(x => !isComputedSpecifiation(x)))
            {
                if (sb.Length == 0)
                {
                    sb.AppendLine(ColumnSql[item.Name]);
                }
                else
                {
                    sb.AppendLine($",{ColumnSql[item.Name]}");
                }
            }
            return sb.ToString();
        }
        public bool CheckKeySize()
        {
            int total = 0;
            foreach (var item in model.Columns().Where(t => t.Properties.isPrimary))
            {
                if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "bigint")
                {
                    total += 8;
                }
                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "int")
                {
                    total += 4;
                }
                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "smallint")
                {
                    total += 2;
                }

                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "tinyint")
                {
                    total += 1;
                }

                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "decimal")
                {
                    total += 17;
                }

                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "float")
                {
                    total += 8;
                }

                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "money")
                {
                    total += 8;
                }
                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "smallmoney")
                {
                    total += 4;
                }

                else if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "uniqueidentifier")
                {
                    total += 41;
                }
                else
                {
                    if (item.Length == "MAX")
                    {
                        total += 4000;
                    }
                    else
                    {
                        if (DataTypeConverter.GetSqlName(item.DataType).ToLower() == "nvarchar")
                        {
                            total += int.Parse(item.Length.Split(",".ToCharArray())[0]) * 2;
                        }
                        else
                        {
                            total += int.Parse(item.Length.Split(",".ToCharArray())[0]);
                        }

                    }

                }
            }

            if (total < 501)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
