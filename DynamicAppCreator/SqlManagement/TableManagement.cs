using DAC.core.abstractions;
using DAC.core.models;
using DAC.kernel;
using DAC.kernel.security;
using DynamicAppCreator.Data;
using DynamicAppCreator.Managers;
using DynamicAppCreator.SqlManagement.models;
using Microsoft.SqlServer.Management.Common;

namespace DynamicAppCreator.SqlManagement
{
    public class TableManagement
    {
        private readonly ApplicationDbContext dbContext;
        private readonly SystemProccess systemProccess;
        public string TablePrefix { get; set; } = "";
        public TableManagement(ApplicationDbContext dbContext, SystemProccess systemProccess)
        {
            this.dbContext = dbContext;
            this.systemProccess = systemProccess;
        }
        private string FindNewTableName(int NextCount = 0)
        {

            var count = dbContext.SqlTables.Count(x => x.Name.StartsWith(this.TablePrefix + "tbl_"));
            var newName = TablePrefix + "tbl_" + (count + NextCount);
            if (dbContext.SqlTables.Any(x => x.Name == newName))
            {
                NextCount++;
                return FindNewTableName(NextCount);
            }
            else
            {
                return newName;
            }
        }

        private string FindColumnName(int NextCount, List<Columns> cols)
        {

            var count = cols.Count(x => x.Name.StartsWith("field_"));
            var newName = "field_" + (count + NextCount);
            if (cols.Any(x => x.Name == newName))
            {
                NextCount++;
                return FindColumnName(NextCount, cols);
            }
            else
            {
                return newName;
            }
        }
        public ApiResult<IEnumerable<SqlTables>> List(long database)
        {

            var exist = dbContext.SqlTables.Where(x => x.Database == database);
            return new ApiResult<IEnumerable<SqlTables>>
            {
                state = true,
                Result = exist,
            };

        }
        public ApiResult<SqlTables> AddTable(CreateTableInputModel model)
        {
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == model.Database);
            if (databaseModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            var newName = FindNewTableName(0);
            model.Name = newName;

            if (model.Columns.Count() == 0)
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.requireminonecolumn)" };
            }

            if (!model.Columns.Any(t => t.Properties.isPrimary))
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.requiredPrimaryKey)" };
            }

            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));


            MigrationBuilder mb = new MigrationBuilder(new CreateSqlTable()
            {
                AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.TableAutomations),
                ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.Columns),
                Database = model.Database,
                Description = model.Description,
                Name = model.Name,
                Schema = model.Schema,
                Type = model.Type,
            });

            try
            {
                var sql = mb.CreateMigrationSql(new List<Columns>());
                server.ConnectionContext.Connect();
                server.Databases[databaseModel.Name].ExecuteNonQuery(sql);
                server.ConnectionContext.Disconnect();
                var newTableToDb = new SqlTables()
                {
                    AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.TableAutomations),
                    ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.Columns),
                    Database = model.Database,
                    Description = model.Description,
                    Name = model.Name,
                    Schema = model.Schema,
                    Type = model.Type,
                    Existing = false,
                    Version = 0
                };
                dbContext.SqlTables.Add(newTableToDb).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                dbContext.SaveChanges();

                systemProccess.AddLog("TableManagement", DAC.core.enums.LogTypesEnum.Created, newTableToDb);

                var binaryData = BinaryData.FromObjectAsJson(newTableToDb).ToMemory();

                return new ApiResult<SqlTables>() { state = false, Result = newTableToDb };
            }
            catch (Exception ex)
            {
                return new ApiResult<SqlTables>() { state = false, Message = ex.Message };
            }


        }
        public ApiResult<SqlTables> UpdateTable(UpdateTableInputModel model)
        {
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == model.Database);
            if (databaseModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            if (model.Columns.Count() == 0)
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.requireminonecolumn)" };
            }

            if (!model.Columns.Any(t => t.Properties.isPrimary))
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.requiredPrimaryKey)" };
            }

            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));


            var oldSettings = dbContext.SqlTables.FirstOrDefault(x => x.Id == model.Id);


            model.Columns.ForEach(q =>
            {
                if (string.IsNullOrEmpty(q.Name))
                {
                    q.Name = this.FindColumnName(1, model.Columns);
                }
            });

            MigrationBuilder mb = new MigrationBuilder(new CreateSqlTable()
            {
                AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.TableAutomations),
                ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.Columns),
                Database = model.Database,
                Description = model.Description,
                Name = model.Name,
                Schema = model.Schema,
                Type = model.Type,
            });

            try
            {
                oldSettings.Columns().ForEach(q =>
                {
                    if (string.IsNullOrEmpty(q.Name))
                    {
                        q.Name = this.FindColumnName(1, oldSettings.Columns());
                    }
                });



                var sql = mb.CreateMigrationSql(oldSettings.Columns());
                server.ConnectionContext.Connect();
                server.Databases[databaseModel.Name].ExecuteNonQuery(sql);
                server.ConnectionContext.Disconnect();

                oldSettings.AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.TableAutomations);
                oldSettings.ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(model.Columns);

                oldSettings.Version++;
                oldSettings.Description = model.Description;
                dbContext.SqlTables.Update(oldSettings).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                dbContext.SaveChanges();
                systemProccess.AddLog("TableManagement", DAC.core.enums.LogTypesEnum.Modified, oldSettings);

                return new ApiResult<SqlTables>() { state = false, Result = oldSettings };
            }
            catch (Exception ex)
            {
                return new ApiResult<SqlTables>() { state = false, Message = ex.Message };
            }


        }
        public ApiResult<SqlTables> AddExistingTable(CreateExistingTableInputModel model)
        {
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == model.Database);
            if (databaseModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }




            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var existed = server.Databases[databaseModel.Name].Tables[model.Name];

            if (existed == null)
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddExistingTable.error.tablentfound)" };
            }

            List<Columns> columns = new List<Columns>();
            for (int i = 0; i < existed.Columns.Count; i++)
            {
                var exist = existed.Columns[i];

                columns.Add(new Columns()
                {
                    ColumnOrder = i,
                    ComputedColumnSpecification = new ComputedColumnSpecification()
                    {
                        Formula = exist.ComputedText,
                        isPersisted = exist.Computed,
                    },
                    DataType = exist.DataType.SqlDataType,
                    Description = exist.Name,
                    Name = exist.Name,
                    Type = 0,
                    Properties = new ColumnProperties()
                    {
                        AllowNull = exist.Nullable,
                        isPrimary = exist.InPrimaryKey,
                        isIdentity = exist.Identity,
                        isUnique = exist.IsForeignKey,
                        AllowInsert = true,
                        AllowUpdate = true,
                        isIndex = exist.IsFullTextIndexed
                    },
                    DefaultValueOrBinding = exist.DefaultConstraint?.Text ?? "",
                    Schema = exist.DefaultSchema
                });
            }

            MigrationBuilder mb = new MigrationBuilder(new CreateSqlTable()
            {
                AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(new List<TableAutomation>()),
                ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(columns),
                Database = model.Database,
                Description = model.Description,
                Name = model.Name,
                Schema = "",
                Type = 0,
            });

            try
            {
                //var sql = mb.CreateMigrationSql();
                //server.ConnectionContext.Connect();
                //server.Databases[databaseModel.Name].ExecuteNonQuery(sql);
                //server.ConnectionContext.Disconnect();
                var newTableToDb = new SqlTables()
                {
                    AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(new List<TableAutomation>()),
                    ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(columns),
                    Database = model.Database,
                    Description = model.Description,
                    Name = model.Name,
                    Schema = "",
                    Type = 0,
                    Existing = true
                };
                dbContext.SqlTables.Add(newTableToDb).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                dbContext.SaveChanges();

                systemProccess.AddLog("TableManagement", DAC.core.enums.LogTypesEnum.Created, newTableToDb);

                var binaryData = BinaryData.FromObjectAsJson(newTableToDb).ToMemory();

                return new ApiResult<SqlTables>() { state = false, Result = newTableToDb };
            }
            catch (Exception ex)
            {
                return new ApiResult<SqlTables>() { state = false, Message = ex.Message };
            }

        }
        public ApiResult<SqlTables> AddExistingView(CreateExistingTableInputModel model)
        {
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == model.Database);
            if (databaseModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }




            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var existed = server.Databases[databaseModel.Name].Views[model.Name];

            if (existed == null)
            {
                return new ApiResult<SqlTables>() { state = false, Message = "$lang(TableManagement.AddExistingTable.error.viewnotfound)" };
            }

            List<Columns> columns = new List<Columns>();
            for (int i = 0; i < existed.Columns.Count; i++)
            {
                var exist = existed.Columns[i];

                columns.Add(new Columns()
                {
                    ColumnOrder = i,
                    ComputedColumnSpecification = new ComputedColumnSpecification()
                    {
                        Formula = exist.ComputedText,
                        isPersisted = exist.Computed,
                    },
                    DataType = exist.DataType.SqlDataType,
                    Description = exist.Name,
                    Name = exist.Name,
                    Type = 0,
                    Properties = new ColumnProperties()
                    {
                        AllowNull = exist.Nullable,
                        isPrimary = exist.InPrimaryKey,
                        isIdentity = exist.Identity,
                        isUnique = exist.IsForeignKey,
                        AllowInsert = true,
                        AllowUpdate = true,
                        isIndex = exist.IsFullTextIndexed
                    },
                    DefaultValueOrBinding = exist.DefaultConstraint?.Text ?? "",
                    Schema = exist.DefaultSchema
                });
            }

            try
            {

                var newTableToDb = new SqlTables()
                {
                    AutomationSettings = Newtonsoft.Json.JsonConvert.SerializeObject(new List<TableAutomation>()),
                    ColumnSettings = Newtonsoft.Json.JsonConvert.SerializeObject(columns),
                    Database = model.Database,
                    Description = model.Description,
                    Name = model.Name,
                    Schema = existed.Schema,
                    Type = 1,
                    Existing = true
                };
                dbContext.SqlTables.Add(newTableToDb).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                dbContext.SaveChanges();

                systemProccess.AddLog("TableManagement", DAC.core.enums.LogTypesEnum.Created, newTableToDb);

                var binaryData = BinaryData.FromObjectAsJson(newTableToDb).ToMemory();

                return new ApiResult<SqlTables>() { state = false, Result = newTableToDb };
            }
            catch (Exception ex)
            {
                return new ApiResult<SqlTables>() { state = false, Message = ex.Message };
            }

        }
        public ApiResult<SqlTables> GetTable(Int64 TableID)
        {
            var databaseModel = dbContext.SqlTables.FirstOrDefault(x => x.Id == TableID);
            if (databaseModel != null)
            {
                return new ApiResult<SqlTables>
                {
                    state = true,
                    Result = databaseModel,
                };
            }
            return new ApiResult<SqlTables>
            {
                state = false,
                Message = ""
            };

        }

        public void AddData(Int64 TableID, List<DAC.core.models.Columns> columns, Dictionary<string, object> data)
        {
            var tableModel = dbContext.SqlTables.FirstOrDefault(x => x.Id == TableID);
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == tableModel.Database);
            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            //Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection($"server={serverModel.Server};uid={serverModel.Username};pwd={serverModel.Password.Decrypt()};database={databaseModel.Name};Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                conn.Open();
                System.Text.StringBuilder fields = new System.Text.StringBuilder();
                System.Text.StringBuilder param = new System.Text.StringBuilder();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.Connection = conn;
                    foreach (var item in columns.Where(x => x.Properties.AllowInsert == true && (!x.Properties.isPrimary || !x.Properties.isIdentity)))
                    {
                        if (data.ContainsKey(item.Name))
                        {
                            if (fields.Length == 0)
                            {
                                fields.Append(item.Name);
                                param.Append($"@{item.Name}");
                            }
                            else
                            {
                                fields.Append($",{item.Name}");
                                param.Append($",@{item.Name}");
                            }
                            cmd.Parameters.AddWithValue(item.Name, data[item.Name]);
                        }
                    }
                    cmd.CommandText = $"INSERT INTO {tableModel.Name} ({fields.ToString()}) values ({param.ToString()})";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public System.Data.DataTable GetData(Int64 TableID, List<DAC.core.models.Columns> columns)
        {
            var tableModel = dbContext.SqlTables.FirstOrDefault(x => x.Id == TableID);
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == tableModel.Database);
            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            //Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection($"server={serverModel.Server};uid={serverModel.Username};pwd={serverModel.Password.Decrypt()};database={databaseModel.Name};Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                conn.Open();
                System.Text.StringBuilder fields = new System.Text.StringBuilder();
                System.Text.StringBuilder param = new System.Text.StringBuilder();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.Connection = conn;
                    foreach (var item in columns)
                    {
                        if (fields.Length == 0)
                        {
                            fields.Append(item.Name);
                        }
                        else
                        {
                            fields.Append($",{item.Name}");
                        }
                    }
                    cmd.CommandText = $"SELECT {fields.ToString()} from {tableModel.Name}";
                    using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);
                        return table;

                    }
                }
            }
        }

        public System.Data.DataTable GetData(Int64 TableID, List<string> columns)
        {
            var tableModel = dbContext.SqlTables.FirstOrDefault(x => x.Id == TableID);
            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == tableModel.Database);
            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            //Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection($"server={serverModel.Server};uid={serverModel.Username};pwd={serverModel.Password.Decrypt()};database={databaseModel.Name};Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                conn.Open();
                System.Text.StringBuilder fields = new System.Text.StringBuilder();
                System.Text.StringBuilder param = new System.Text.StringBuilder();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.Connection = conn;
                    foreach (var item in columns)
                    {
                        if (fields.Length == 0)
                        {
                            fields.Append(item);
                        }
                        else
                        {
                            fields.Append($",{item}");
                        }
                    }
                    cmd.CommandText = $"SELECT {fields.ToString()} from {tableModel.Name}";
                    using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);
                        return table;

                    }
                }
            }
        }
    }
}
