using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.identity
{
    public class IdentityHelper
    {
        //private readonly IHttpContextAccessor httpContextAccessor;

        //public IdentityHelper(IHttpContextAccessor httpContextAccessor)
        //{
        //    this.httpContextAccessor = httpContextAccessor;
        //}

        //public string Name
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        //        {
        //            return httpContextAccessor.HttpContext.User.Identity.Name;
        //        }
        //        return String.Empty;
        //    }
        //}
        //public string Id
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.GroupSid))
        //        {
        //            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.GroupSid).Value;
        //        }
        //        return String.Empty;

        //    }
        //}

        //public string Email
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Email))
        //        {
        //            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value;
        //        }
        //        return String.Empty;

        //    }
        //}

        //public string Type
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.PrimaryGroupSid))
        //        {
        //            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.PrimaryGroupSid).Value;
        //        }
        //        return String.Empty;

        //    }
        //}

        //public string Locality
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Locality))
        //        {
        //            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Locality).Value;
        //        }
        //        return String.Empty;

        //    }
        //}

        //public long Role
        //{
        //    get
        //    {
        //        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Role))
        //        {
        //            return long.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value);
        //        }
        //        return -1;

        //    }
        //}

    }
}
