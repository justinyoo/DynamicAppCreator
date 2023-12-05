using DAC.core.identity;
using DynamicAppCreator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicAppCreator.Managers
{
    public class GeneralAccess
    {
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
        public bool edit { get; set; }
    }

    public class DetailAccess : GeneralAccess
    {
        public string scope { get; set; }
    }

    public class SimpleAccess : GeneralAccess
    {
        public string scope { get; set; }
        public bool allowed { get; set; }
    }
    public class PermissionManager
    {
        private readonly ApplicationDbContext kernelDbContext;
        private readonly IdentityHelper identityHelper;

        public PermissionManager(ApplicationDbContext kernelDbContext, IdentityHelper identityHelper)
        {
            this.kernelDbContext = kernelDbContext;
            this.identityHelper = identityHelper;
        }
        public void createGroup() { }
        public void copyGroup() { }
        public void deleteGroup() { }
        public void updateGroup() { }

        public void GetAccess(string SectionName, string SectionKey)
        {
            //if (kernelDbContext.PermissionGroups.Any(x => x.Name == identityHelper.Email))
            //{
            //    // kernelDbContext.Permissions.FirstOrDefault(x => x.UserOrGroup == group.Id.ToString() || x.UserOrGroup == UserOrGroup);
            //}
            //else if (kernelDbContext.PermissionGroups.Any(x => x.Id == identityHelper.Role))
            //{

            //}
        }

        public GeneralAccess GeneralAccess(string UserOrGroup, string SectionName, string SectionKey)
        {
            return new GeneralAccess();
        }

        public List<SimpleAccess> AccessingViewables(string UserOrGroup, string SectionName, string SectionKey)
        {
            return new List<SimpleAccess>();
        }

        public List<SimpleAccess> AccessingEditables(string UserOrGroup, string SectionName, string SectionKey)
        {
            return new List<SimpleAccess>();
        }

        public List<SimpleAccess> AccessingCreatables(string UserOrGroup, string SectionName, string SectionKey)
        {
            return new List<SimpleAccess>();
        }

        public List<SimpleAccess> AccessingActions(string UserOrGroup, string SectionName, string SectionKey)
        {
            return new List<SimpleAccess>();
        }
    }
}
