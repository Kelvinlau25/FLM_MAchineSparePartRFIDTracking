using PAB_NewAquarium.DAL;
using System.Web.Mvc;
using System.Web.Routing;

namespace PAB_NewAquarium.Helpers
{
    public class BaseController : Controller
    {
        public readonly CommonFunction common;
        public readonly ErrorLogSys errorLog;
        public readonly ProxyHelper proxy;
        public string username;

        public BaseController()
        {
            common = new CommonFunction();
            errorLog = new ErrorLogSys();
            proxy = new ProxyHelper();
        }

        protected ACL_UserObj AclUser
        {
            get
            {
                return Session["AclUser"] as ACL_UserObj;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (AclUser == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "SessionExpired" }));
            }
            else
            {
                username = $"{AclUser.USER_ID} - {AclUser.EMP_NAME}";
            }
        }
    }
}