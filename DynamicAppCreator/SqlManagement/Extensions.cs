using DynamicAppCreator.Managers;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppCreator.SqlManagement
{
    public static class Extensions
    {
        public static IServiceCollection AddPluginServices(this IServiceCollection services)
        {

            services.AddScoped<ServerManagement>();
            services.AddScoped<DatabaseManagement>();
            services.AddScoped<TableManagement>();
            services.AddScoped<SystemProccess>();
            services.AddScoped<DynamicAppCreator.ModuleManagement.ModuleManagement>();
            services.AddScoped<DynamicAppCreator.SqlManagement.DataProcessing.DataProcessing>();
            //
            //services.AddDbContext<KernelDbContext>(options =>
            //{ }
            ////    options.UseSqlServer(connectionString)); 
            //);
            return services;
        }


    }
}
