using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace FILM_Sparepart_MVC.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string redirectTo = "~/Home/Index";

            if (!context.HttpContext.Session.Keys.Contains("AclUser"))
            {
                if (!string.IsNullOrEmpty(context.HttpContext.Request.Path))
                {
                    redirectTo = $"{redirectTo}?ReturnUrl={Uri.EscapeDataString(context.HttpContext.Request.Path)}";
                }

                context.Result = new RedirectResult(redirectTo);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}