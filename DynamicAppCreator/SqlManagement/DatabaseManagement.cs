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
    public class DatabaseManagement
    {
        private readonly ApplicationDbContext dbContext;
        private readonly SystemProccess systemProccess;
        public DatabaseManagement(ApplicationDbContext dbContext, SystemProccess systemProccess)
        {
            this.dbContext = dbContext;
            this.systemProccess = systemProccess;
        }
        public ApiResult<IEnumerable<SqlDatabases>> List(long server)
        {

            var exist = dbContext.SqlServers.FirstOrDefault(x => x.Id == server);

            if (exist != null)
            {
                return new ApiResult<IEnumerable<SqlDatabases>>
                {
                    state = true,
                    Result = dbContext.SqlDatabases.Where(t => t.ServerID == exist.Id),
                    Message = $"{exist.Name} server databases fetched."
                };
            }

            return new ApiResult<IEnumerable<SqlDatabases>>
            {
                state = false,
                Message = "Sql Server was not found."
            };
        }

        public ApiResult<SqlDatabases> GetDatabase(long database)
        {

            var exist = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == database);

            if (exist != null)
            {
                return new ApiResult<SqlDatabases>
                {
                    state = true,
                    Result = dbContext.SqlDatabases.FirstOrDefault(t => t.Id == exist.Id),
                    Message = $"{exist.Name} table read details."
                };
            }

            return new ApiResult<SqlDatabases>
            {
                state = false,
                Message = "Table was not found."
            };
        }
        public ApiResult<SqlDatabases> AddDatabase(CreateDatabaseInputModel model)
        {

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == model.Server);
            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var db = new Microsoft.SqlServer.Management.Smo.Database(server, model.Name);
            db.Create();
            var item = this.dbContext.SqlDatabases.Add(new SqlDatabases()
            {
                Description = model.Description,
                Name = model.Name,
                ServerID = model.Server,
                Type = (int)model.Type,

            });
            item.State = Microsoft.EntityFrameworkCore.EntityState.Added;
            dbContext.SaveChanges();
            systemProccess.AddLog("DatabaseManagement.AddDatabase", DAC.core.enums.LogTypesEnum.Created, item.Entity);


            return new ApiResult<SqlDatabases>() { state = true, Result = item.Entity };
        }
        public ApiResult<SqlDatabases> UpdateDatabase(UpdateDatabaseInputModel model)
        {

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == model.Server);
            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var oldDb = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == model.Id);
            if (oldDb != null)
            {
                var db = new Microsoft.SqlServer.Management.Smo.Database(server, oldDb.Name);
                db.Rename(model.Name);
                oldDb.Name = model.Name;
                oldDb.Description = model.Description;
                dbContext.Entry(oldDb).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                dbContext.SaveChanges();
                systemProccess.AddLog("DatabaseManagement.UpdateDatabase", DAC.core.enums.LogTypesEnum.Modified, oldDb);
                return new ApiResult<SqlDatabases>() { state = true, Result = oldDb };
            }

            return new ApiResult<SqlDatabases>() { state = false, Message = "OldDb NotFound" };
        }
        public ApiResult<SqlDatabases> AddExistingDatabase(CreateDatabaseInputModel model)
        {

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == model.Server);
            if (serverModel != null)
            {
                Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

                var x = server.Databases[model.Name];
                if (x != null)
                {
                    if (dbContext.SqlDatabases.Any(x => x.Name == model.Name))
                    {
                        return new ApiResult<SqlDatabases>() { state = false, Message = "$lang(databasemanagement.adddatabase.error.databasealreadyadded)" };
                    }
                    var item = this.dbContext.SqlDatabases.Add(new SqlDatabases()
                    {
                        Description = model.Description,
                        Name = model.Name,
                        ServerID = model.Server,
                        Type = (int)model.Type,
                    });
                    item.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    dbContext.SaveChanges();
                    systemProccess.AddLog("DatabaseManagement.AddExistingDatabase", DAC.core.enums.LogTypesEnum.Created, item.Entity);

                    return new ApiResult<SqlDatabases>() { state = true, Result = item.Entity };
                }

                return new ApiResult<SqlDatabases>() { state = false, Message = "$lang(databasemanagement.adddatabase.error.databasenotfound)" };

            }


            return new ApiResult<SqlDatabases>() { state = false, Message = "$lang(databasemanagement.adddatabase.error.servernotfound)" };

        }
        public ApiResult<IEnumerable<SqlDatabases>> Tables(long Database)
        {

            var exist = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == Database);

            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection());

            Microsoft.SqlServer.Management.Smo.Table t = new Microsoft.SqlServer.Management.Smo.Table(server.Databases[0], "tableName", "dbo");
            t.Create();

            Microsoft.SqlServer.Management.Smo.Column c = new Microsoft.SqlServer.Management.Smo.Column(t, "field_1", Microsoft.SqlServer.Management.Smo.DataType.BigInt, false);
            c.Create();

            if (exist != null)
            {
                return new ApiResult<IEnumerable<SqlDatabases>>
                {
                    state = true,
                    Result = dbContext.SqlDatabases.Where(t => t.ServerID == exist.Id),
                };
            }

            return new ApiResult<IEnumerable<SqlDatabases>>
            {
                state = false,
                Message = ""
            };
        }
        public ApiResult<object> BaseTables(long Database)
        {

            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == Database);
            if (databaseModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var tbls = server.Databases[databaseModel.Name].Tables;

            List<object> tables = new List<object>();
            for (int i = 0; i < tbls.Count; i++)
            {
                var tbl = tbls[i];

                if (!dbContext.SqlTables.Any(x => x.Database == databaseModel.Id && x.Name == tbl.Name))
                {
                    tables.Add(new
                    {
                        name = tbls[i].Name,
                        type = 0
                    });
                }
            }

            return new ApiResult<object>() { state = true, Result = tables };
        }

        public ApiResult<object> BaseViews(long Database)
        {

            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == Database);
            if (databaseModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            List<object> tables = new List<object>();

            var views = server.Databases[databaseModel.Name].Views;
            for (int i = 0; i < views.Count; i++)
            {
                var view = views[i];
                if (!view.IsSystemObject && !dbContext.SqlTables.Any(x => x.Database == databaseModel.Id && x.Name == view.Name))
                {
                    tables.Add(new
                    {
                        name = views[i].Name,
                        type = 1
                    });
                }
            }
            return new ApiResult<object>() { state = true, Result = tables };
        }

        public ApiResult<object> StoredProcedures(long Database)
        {

            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == Database);
            if (databaseModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var tbls = server.Databases[databaseModel.Name].StoredProcedures;
            List<object> tables = new List<object>();

            var res = server.Databases[databaseModel.Name].ExecuteWithResults("SELECT name FROM sys.procedures WHERE type = 'P';");
            foreach (System.Data.DataTable _table in res.Tables)
            {
                foreach (System.Data.DataRow _row in _table.Rows)
                {
                    Microsoft.SqlServer.Management.Smo.StoredProcedure item = server.Databases[databaseModel.Name].StoredProcedures[_row[0].ToString()];

                    if (item != null && !item.IsSystemObject)
                    {
                        var prms = new List<object>();

                        var p = item.Parameters;
                        for (int pi = 0; pi < p.Count; pi++)
                        {
                            prms.Add(new
                            {
                                p[pi].Name,
                                p[pi].DataType,
                                p[pi].DefaultValue,
                                p[pi].IsReadOnly
                            });
                        }
                        tables.Add(new
                        {
                            name = item.Name,
                            parameters = prms,
                        });
                    }

                }
            }


            //for (int i = 0; i < tbls.Count; i++)
            //{
            //    var tbl = tbls[i];
            //    if (!tbl.IsSystemObject)
            //    {
            //        var prms = new List<object>();

            //        var p = tbl.Parameters;
            //        for (int pi = 0; pi < p.Count; pi++)
            //        {
            //            prms.Add(new
            //            {
            //                p[pi].Name,
            //                p[pi].DataType,
            //                p[pi].DefaultValue,
            //                p[pi].IsReadOnly
            //            });
            //        }
            //        tables.Add(new
            //        {
            //            name = tbl.Name,
            //            parameters = prms,
            //        });
            //    }
            //}

            return new ApiResult<object>() { state = true, Result = tables };
        }
        public ApiResult<object> Functions(long Database)
        {

            var databaseModel = dbContext.SqlDatabases.FirstOrDefault(x => x.Id == Database);
            if (databaseModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.databasenotfound)" }; }

            var serverModel = dbContext.SqlServers.FirstOrDefault(x => x.Id == databaseModel.ServerID);
            if (serverModel == null) { return new ApiResult<object>() { state = false, Message = "$lang(TableManagement.AddTable.error.servernotfound)" }; }


            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(serverModel.Server, serverModel.Username, serverModel.Password.Decrypt()));

            var tbls = server.Databases[databaseModel.Name].StoredProcedures;
            List<object> tables = new List<object>();

            var res = server.Databases[databaseModel.Name].ExecuteWithResults("SELECT name FROM sys.procedures WHERE type = 'P';");
            foreach (System.Data.DataTable _table in res.Tables)
            {
                foreach (System.Data.DataRow _row in _table.Rows)
                {
                    Microsoft.SqlServer.Management.Smo.StoredProcedure item = server.Databases[databaseModel.Name].StoredProcedures[_row[0].ToString()];

                    if (item != null && !item.IsSystemObject)
                    {
                        var prms = new List<object>();

                        var p = item.Parameters;
                        for (int pi = 0; pi < p.Count; pi++)
                        {
                            prms.Add(new
                            {
                                p[pi].Name,
                                p[pi].DataType,
                                p[pi].DefaultValue,
                                p[pi].IsReadOnly
                            });
                        }
                        tables.Add(new
                        {
                            name = item.Name,
                            parameters = prms,
                        });
                    }

                }
            }


            return new ApiResult<object>() { state = true, Result = tables };
        }

        public ApiResult<object> CreateFunction(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }

        public ApiResult<object> Update(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }

        public ApiResult<object> CreateProcedure(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }

        public ApiResult<object> UpdateProcedure(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }

        public ApiResult<object> CreateDataType(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }

        public ApiResult<object> UpdateDataType(CreateFunctionInput model)
        {
            return new ApiResult<object>() { };
        }
    }
}
