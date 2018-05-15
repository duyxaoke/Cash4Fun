using CashMe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies["userInfo"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Home");
            }
        }

        protected override bool DisableAsyncSupport
        {
            get
            {
                return true;
            }
        }
    }
}