using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;   
using System.Web.Mvc;

namespace VirtualHIE
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["UserID"] == null)
            {
               // ViewBag.test = "SessionTimedOut";
                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

    }
}