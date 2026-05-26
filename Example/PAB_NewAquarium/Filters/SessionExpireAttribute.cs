using PAB_NewAquarium.Helpers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Web.Routing;
using System.IdentityModel.Tokens.Jwt;

namespace PAB_NewAquarium.Filters
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            var aclUser = context.Session["AclUser"] as ACL_UserObj;

            if (aclUser == null)
            {
                // Redirect to session expired page if session is null
                filterContext.Result = RedirectTo("Home", "SessionExpired");
                return;
            }
            else
            {
                var controller = filterContext.Controller.ControllerContext.RouteData.Values["Controller"].ToString();
                var action = filterContext.Controller.ControllerContext.RouteData.Values["Action"].ToString();
                var acl = aclUser.ACCESS_LIST;
                bool hasAccess = acl?.Any(x => x.RESOURCE_CONTROLLER == controller && x.RESOURCE_VIEW == action) ?? false;

                // If user doesn't have access, redirect to access denied page
                if (!hasAccess)
                {
                    filterContext.Result = RedirectTo("Home", "AccessDenied");
                    return;
                }
            }  

            base.OnActionExecuting(filterContext);
        }

        private RedirectToRouteResult RedirectTo(string controller, string action)
        {
            return new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", controller },
                { "action", action }
            });
        }
    }
}