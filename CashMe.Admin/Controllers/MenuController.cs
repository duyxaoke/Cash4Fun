using CashMe.Service;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    public class MenuController : Controller
    {
        #region Ctor
        public MenuController()
        {
            
        }
        #endregion

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WalletServices _walletServices = new WalletServices();

        public MenuController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Menu
        public PartialViewResult Header()
        {
            var modelUser = new RegisterViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var getId = User.Identity;
                var user = UserManager.FindById(User.Identity.GetUserId());
                modelUser = user.MapTo();
            }
            ViewBag.MainView = _walletServices.getMainView();
            return PartialView(modelUser);
        }
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        public ActionResult Sidebar()
        {
            return PartialView();
        }
    }
}