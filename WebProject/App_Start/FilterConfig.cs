using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace WebProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizeUserFilter());
        }
    }
    public class AuthorizeUserFilter : AuthorizeAttribute
    {

    }
}
