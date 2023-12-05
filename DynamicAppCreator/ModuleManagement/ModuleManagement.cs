using DAC.core.models;
using DynamicAppCreator.Data;
using DynamicAppCreator.SqlManagement;

namespace DynamicAppCreator.ModuleManagement
{
    public class ModuleManagement
    {
        private readonly ApplicationDbContext dbContext;
        private readonly TableManagement tableManagement;

        public ModuleManagement(ApplicationDbContext dbContext, TableManagement tableManagement)
        {
            this.dbContext = dbContext;
            this.tableManagement = tableManagement;
        }
        public IEnumerable<Modules> GetMenuModules()
        {
            return this.dbContext.Modules.Where(x => x.ShowInMenu == true);
        }

        public IEnumerable<Modules> GetAllModules()
        {
            return this.dbContext.Modules;
        }

        public Modules GetModule(long id)
        {
            return this.dbContext.Modules.FirstOrDefault(q => q.Id == id);
        }

        public List<Columns> GetModuleColumns(long id)
        {
            var module = this.dbContext.Modules.FirstOrDefault(q => q.Id == id);
            var table = tableManagement.GetTable(module.Table);
            return table.Result.Columns();

        }
    }
}
