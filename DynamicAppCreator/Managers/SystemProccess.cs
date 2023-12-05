using DAC.core.enums;
using DynamicAppCreator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicAppCreator.Managers
{
    public class SystemProccess
    {
        private readonly ApplicationDbContext kernelDb;

        public SystemProccess(ApplicationDbContext kernelDb)
        {
            this.kernelDb = kernelDb;
        }

        public void AddLog(string scope, LogTypesEnum type, object Data)
        {

            kernelDb.AppLogger.Add(new DAC.kernel.models.AppLogger()
            {
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(Data),
                ModifiedDate = DateTime.Now,
                Owner = "",
                Scope = scope,
                Type = type
            }).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            kernelDb.SaveChanges();

        }
    }
}
