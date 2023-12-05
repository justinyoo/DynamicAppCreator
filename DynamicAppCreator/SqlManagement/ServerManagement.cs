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
    public class ServerManagement
    {
        private readonly ApplicationDbContext managementDb;
        private readonly SystemProccess systemProccess;

        public ServerManagement(ApplicationDbContext managementDb, SystemProccess systemProccess)
        {
            this.managementDb = managementDb;
            this.systemProccess = systemProccess;
        }
        public ApiResult<SqlServers> AddServer(CreateServerInputModel model)
        {

            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(model.Server, model.Username, model.Password));

            try
            {
                server.ConnectionContext.Connect();
                server.ConnectionContext.Disconnect();
                var entity = managementDb.SqlServers.Add(new SqlServers()
                {
                    DefaultDb = model.DefaultDb,
                    Username = model.Username,
                    Password = model.Password.Crypt(),
                    Server = model.Server,
                    Description = model.Description,
                    Name = model.Name,
                });
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                managementDb.SaveChanges();

                systemProccess.AddLog("ServerManagement.AddServer", DAC.core.enums.LogTypesEnum.Created, entity.Entity);


                return new ApiResult<SqlServers>()
                {
                    Result = entity.Entity,
                    state = true,
                    Message = "$lang(sqlmanagement.servers.success.add)",
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<SqlServers>()
                {
                    Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                    state = false
                };
            }
        }
        public ApiResult<List<SqlServers>> GetServers()
        {

            return new ApiResult<List<SqlServers>>()
            {
                Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                state = true,
                Result = managementDb.SqlServers.ToList()
            };
        }

        public ApiResult<SqlServers> GetServer(long id)
        {

            return new ApiResult<SqlServers>()
            {
                Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                state = true,
                Result = managementDb.SqlServers.FirstOrDefault(t => t.Id == id)
            };
        }
        public ApiResult<Microsoft.SqlServer.Management.Smo.LoginCollection> Logins(long id)
        {
            if (managementDb.SqlServers.Any(t => t.Id == id))
            {
                var model = managementDb.SqlServers.FirstOrDefault(x => x.Id == id);
                Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(model.Server, model.Username, model.Password));
                return new ApiResult<Microsoft.SqlServer.Management.Smo.LoginCollection>()
                {
                    Result = server.Logins,
                    state = true
                };
            }
            return new ApiResult<Microsoft.SqlServer.Management.Smo.LoginCollection>() { state = false };
        }
        public ApiResult<object> RemoveServer(long id) { return new ApiResult<object>(); }
        public ApiResult<SqlServers> UpdateServer(UpdateServerInputModel model)
        {
            var exist = managementDb.SqlServers.FirstOrDefault(x => x.Id == model.Id);


            try
            {
                if (exist != null)
                {
                    var pass = "";
                    if (exist.Password == model.Password)
                    {
                        pass = model.Password.Decrypt();
                    }
                    else
                    {
                        pass = model.Password;
                        exist.Password = model.Password.Crypt();

                    }

                    exist.DefaultDb = model.DefaultDb;
                    exist.Description = model.Description;
                    exist.Name = model.Name;
                    exist.Name = model.Name;
                    exist.Server = model.Server;

                    Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(model.Server, model.Username, pass));
                    server.ConnectionContext.Connect();
                    server.ConnectionContext.Disconnect();
                    var entity = managementDb.SqlServers.Entry(exist);
                    entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    managementDb.SaveChanges();
                    systemProccess.AddLog("ServerManagement.UpdateServer", DAC.core.enums.LogTypesEnum.Modified, entity.Entity);



                    return new ApiResult<SqlServers>()
                    {
                        Result = entity.Entity,
                        state = true,
                        Message = "$lang(sqlmanagement.servers.success.update)",
                    };
                }
                return new ApiResult<SqlServers>()
                {
                    Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                    state = false
                };

            }
            catch (Exception ex)
            {
                return new ApiResult<SqlServers>()
                {
                    Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                    state = false
                };
            }

        }
        public ApiResult<object> GetServerProperties(long id)
        {
            var model = managementDb.SqlServers.FirstOrDefault(x => x.Id == id);

            try
            {
                if (model != null)
                {

                    Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(model.Server, model.Username, model.Password.Decrypt()));
                    return new ApiResult<object>()
                    {
                        Message = "$lang(sqlmanagement.servers.success.getproperties)",
                        state = true,
                        Result = new
                        {
                            properties = new
                            {
                                server.AuditLevel,
                                server.AvailabilityGroups,
                                server.BackupDirectory,
                                server.BuildClrVersion,
                                server.BuildNumber,
                                server.ClusterName,
                                server.Collation,
                                server.CollationID,
                                server.DatabaseEngineEdition,
                                server.DatabaseEngineType,
                                server.DefaultFile,
                                server.DefaultLog,
                                server.Edition,
                                server.Name,
                                server.Version,
                                server.ComparisonStyle,
                                server.ErrorLogPath,
                                server.FilestreamLevel,
                                server.FilestreamShareName,
                                server.HostPlatform,
                                server.InstanceName,
                                server.IsClustered,
                                server.IsFullTextInstalled,
                                server.Information.Processors,
                                server.Information.State,
                                server.PhysicalMemory,
                                server.PhysicalMemoryUsageInKB,
                                server.MailProfile
                            },
                            details = new
                            {
                                model.Name,
                                model.Username,
                                model.Password,
                                model.DefaultDb,
                                model.Description
                            }
                        }
                    };

                }
                return new ApiResult<object>()
                {
                    Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                    state = false
                };

            }
            catch (Exception ex)
            {
                return new ApiResult<object>()
                {
                    Message = "$lang(sqlmanagement.servers.error.connecttoserver)",
                    state = false
                };
            }

        }
        public ApiResult<object> KillAllProcesses(long id, string Database)
        {

            var model = managementDb.SqlServers.FirstOrDefault(x => x.Id == id);
            if (model != null)
            {
                Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(model.Server, model.Username, model.Password.Decrypt()));


                try
                {
                    server.KillAllProcesses(Database);


                }
                catch (Exception ex)
                {
                    return new ApiResult<object>() { state = false, Message = ex.Message };
                }

                return new ApiResult<object>();
            }
            return new ApiResult<object>() { state = false, Message = "$lang(sqlmanagement.servers.error.servernotfound)" };
        }
    }
}
