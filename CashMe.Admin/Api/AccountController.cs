using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using CashMe.Admin.Models;
using CashMe.Data.DAL;
using System.Security.Claims;

namespace CashMe.Admin.Api
{
    public class AccountController : ApiController
    {
        #region Ctor
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET api/Account/GetCurrentUser          
        public UserModel GetCurrentUser()
        {
            if (UserManager == null)
                return new UserModel();
            var model = new UserModel();

            //var identity = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identity.Claims;
            //var user = UserManager.FindById(tmp);
            //if (user != null)
            //{
            //    model.UserName = user.UserName;
            //    model.Email = user.Email;
            //    model.PhoneNumber = user.PhoneNumber;
            //}

            return model;
        }
        #endregion

    }
}
