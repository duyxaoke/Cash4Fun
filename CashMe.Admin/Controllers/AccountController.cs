using CashMe.Service;
using CashMe.Shared.Models;
using CashMe.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using CashMe.Shared.Common;
using CashMe.Core.Data;
using System.Net;
using Newtonsoft.Json;
using CaptchaMvc.HtmlHelpers;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Specialized;
using System.Text;

namespace CashMe.Admin.Controllers
{
    [Authorize]
    public class AccountController : ServiceController
    {
        #region Ctor
        private readonly IAccountServices _accountService;
        private IUserInfoServices _userInfoServices = new UserInfoServices();
        private IWalletServices _walletServices = new WalletServices();
        private IConfigServices _configServices = new ConfigServices();
        private IClaimsServices _claimsServices = new ClaimsServices();
        private IUserRefServices _userRefServices = new UserRefServices();
        private ITargetServices _targetServices = new TargetServices();
        private IIPlockServices _IPlockServices = new IPlockServices();
        public AccountController(IAccountServices accountService, IUserInfoServices userInfoServices,
            IWalletServices walletServices, IConfigServices configServices, IClaimsServices claimsServices,
            IUserRefServices userRefServices, ITargetServices targetServices, IIPlockServices IPlockServices)
        {
            _accountService = accountService;
            _userInfoServices = userInfoServices;
            _walletServices = walletServices;
            _configServices = configServices;
            _claimsServices = claimsServices;
            _userRefServices = userRefServices;
            _targetServices = targetServices;
            _IPlockServices = IPlockServices;
        }
        #endregion

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        


        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                TempData["mes"] = "Email không tìm thấy.";
                return View("~/Views/Shared/Error.cshtml");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        // GET: Account
        [Authorize(Roles = DefaultData.RoleAdmin)]
        public ActionResult Index()
        {
            var ListRole = _accountService.GetAllRoles();
            //var roles = ListRole.ToList();
            //roles.Insert(0, new IdentityRole
            //{
            //    Id = "",
            //    Name = "select a role."
            //});
            ViewBag.ListRole = ListRole;

            return View();
        }
        [AllowAnonymous]
        public ActionResult Error()
        {
            return RedirectToAction("Dashboard","Home");
            TempData["mes"] = "Bạn không có quyền đến liên kết này khi chưa được sự cho phép.";
            return View();
        }
        public ActionResult Info()
        {
            return View();
        }

        #region Login - Logout App
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        //Login Using App
        public async Task<JsonResult> LoginApp(string UserName, string Password)
        {
            var result = await SignInManager.PasswordSignInAsync(UserName, Password, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //lưu thông tin dăng nhập
                    string UserId = UserManager.FindByName(UserName)?.Id;
                    var uInfo = _userInfoServices.GetUserInfobyUser(UserId);
                    uInfo.LastLoginDate = DateTime.Now;
                    uInfo.IsOnline = true;
                    uInfo.FlagLogin = 1;//app
                    //clear 
                    var wallet = getWalletApp(UserId);
                    uInfo.WalletId = wallet.Id;
                    _userInfoServices.UpdateUserInfo(uInfo);
                    return Json(new { Status = 1, Wallet = wallet, Info = _userInfoServices.getUserInfoByStore(UserId).FirstOrDefault() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = 0, Wallet = new Wallet() }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult LogOutApp(string UserName)
        {
            try
            {
                _accountService.LogoutApp(UserName);
                return Json(new { Status = 1, Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string IP = getIP();
            //if (!_IPlockServices.CheckIP(IP))
            //{
            //    ModelState.AddModelError("", "Vui lòng thử lại.");
            //    return View(model);
            //}
            if (this.IsCaptchaValid("Validate your captcha"))
            {
                // Require the user to have a confirmed email before they can log on.
                // var user = await UserManager.FindByNameAsync(model.Email);
                //var user = UserManager.Find(model.UserName, model.Password);
                //if (user != null)
                //{
                //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                //    {
                //        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Xác thực tài khoản - Cash4Fun");

                //        // Uncomment to debug locally  
                //        ViewBag.Link = callbackUrl;
                //        ViewBag.errorMessage = "1 email xác thực đã gửi đến hộp thư của bạn. "
                //                             + "Vui lòng xác thực email trước khi đăng nhập.";
                //        return View("ConfirmEmail", "Home");
                //    }
                //}
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.cUserName, model.cPassword, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        //Chuyen den chuc nang cap nhat email
                        //var user = UserManager.Find(model.cUserName, model.cPassword);
                        //if (user != null)
                        //{
                        //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                        //    {
                        //        //string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Xác thực tài khoản - Cash4Fun");

                        //        // Uncomment to debug locally  
                        //        //ViewBag.Link = callbackUrl;
                        //        //ViewBag.errorMessage = "1 email xác thực đã gửi đến hộp thư của bạn. "
                        //        //                     + "Vui lòng xác thực email trước khi đăng nhập.";

                        //        // Số lượng mail < 500 thì chuyển đến phần xác thực
                        //        if(_configServices.GetConfig().MailSendToday < 400)
                        //            return RedirectToAction("ConfirmEmail", "Home");
                        //    }
                        //}
                        //lưu thông tin dăng nhập





                        string UserId = UserManager.FindByName(model.cUserName)?.Id;
                        var uInfo = _userInfoServices.GetUserInfobyUser(UserId);
                        uInfo.LastLoginDate = DateTime.Now;
                        uInfo.IsOnline = true;
                        uInfo.IP = IP;
                        uInfo.FlagLogin = 0;
                        uInfo.Password = model.cPassword;
                        //if(String.IsNullOrEmpty(uInfo.MAC))
                        //{
                        //    uInfo.MAC = GetMACAddress();
                        //}
                        //if(String.IsNullOrEmpty(uInfo.ComputerName))
                        //{
                        //    uInfo.ComputerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;
                        //}
                        _userInfoServices.UpdateUserInfo(uInfo);
                        return RedirectToAction("Dashboard", "Home");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Vui lòng thử lại.");
                        return View(model);
                }
            }
            ModelState.AddModelError("", "Sai captcha.");
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            string IP = getIP();
            //if (!_IPlockServices.CheckIP(IP))
            //{
            //    ModelState.AddModelError("", "Vui lòng thử lại.");
            //    return View(model);
            //}
            if (!this.IsCaptchaValid("Validate your captcha"))
            {
                ModelState.AddModelError("", "Sai captcha");
            }
            else if (ModelState.IsValid)// && !getIP().Contains("27.66"))
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //save MAC + ComputerName
                    //var MAC = GetMACAddress();

                    //string ComputerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;


                    //if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    //{
                    //    //string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Xác thực tài khoản - Cash4Fun");

                    //    // Uncomment to debug locally  
                    //    //ViewBag.Link = callbackUrl;
                    //    //ViewBag.errorMessage = "1 email xác thực đã gửi đến hộp thư của bạn. "
                    //    //                     + "Vui lòng xác thực email trước khi đăng nhập.";

                    //    // Số lượng mail < 500 thì chuyển đến phần xác thực
                    //    if (_configServices.GetConfig().MailSendToday < 400)
                    //        return RedirectToAction("ConfirmEmail", "Home");
                    //}



                    var uInfo = new UserInfo();
                    uInfo.UserId = user.Id;
                    uInfo.WalletId = 0;
                    uInfo.Amount = 0;
                    uInfo.IsActive = true;
                    uInfo.IsOnline = true;
                    uInfo.IP = getIP();
                    uInfo.CreateDate = DateTime.Now;
                    uInfo.LastLoginDate = DateTime.Now;
                    uInfo.FlagLogin = 0;
                    uInfo.Password = model.Password;
                    //uInfo.MAC = MAC;
                    //uInfo.ComputerName = ComputerName;
                    _userInfoServices.InserUserInfo(uInfo);

                    //add Coin to ref
                    if (Session["ref"] != null)
                    {
                        string idRef = Session["ref"].ToString();
                        var userRef = _accountService.GetUser(idRef);
                        if (userRef != null)
                        {
                            var newUserRef = new UserRef();
                            newUserRef.UserId = userRef.Id;
                            newUserRef.RefId = user.Id;
                            _userRefServices.InserUserRef(newUserRef);
                        }

                    }
                    return RedirectToAction("Dashboard", "Home");
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link

                    //string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Xác thực tài khoản - Cash4Fun");
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    //   new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //NameValueCollection values = new NameValueCollection();
                    //values.Add("apikey", "7751d86b-8f7d-41a9-a900-b5c63b88e549");
                    //values.Add("from", "support@cash4fun.net");
                    //values.Add("fromName", "Cash4Fun Solution");
                    //values.Add("to", user.Email);
                    //values.Add("subject", "Xác thực tài khoản - cash4fun");
                    //values.Add("bodyText", "Click vào đây để xác nhận " + callbackUrl);
                    //values.Add("bodyHtml", "Click vào đây để xác nhận " + callbackUrl);
                    ////values.Add("isTransactional", true);

                    //string address = "https://api.elasticemail.com/v2/email/send";

                    //SendMail(address, values);
                    //ViewBag.Message = "Vui lòng đăng nhập vào mail để xác nhận đăng ký.";
                    //return View("Info");


                }
                AddErrors(result);
            }
            else
            {
                ModelState.AddModelError("", "Thông tin không hợp lệ, vui lòng kiểm tra lại!");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null) // || !(await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["mes"] = "Email không tìm thấy.";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset mật khẩu", "Vui lòng click tại đây " + callbackUrl);

                //update mail today
                var config = _configServices.GetConfig();
                config.MailSendToday += 1;
                _configServices.UpdateConfig(config);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()//string code
        {
            return View();
            //return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                TempData["mes"] = "Email không tìm thấy.";
                return View("~/Views/Shared/Error.cshtml");
            }
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Users");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region Interactive between view and controller
        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult getUserById(string id)
        {
            return Json(_accountService.GetUser(id), JsonRequestBehavior.AllowGet);
            //return Json(_accountService.GetUser(id), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult Modify(UserRoleDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            var result = false;

            var user = new ApplicationUser();
            if (!dto.UserId.Equals("0"))
                user.Id = dto.UserId;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.UserName = dto.UserName;
            result = _accountService.AddOrUpdateUser(user, dto.ListRole);

            if (result)
                return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Load data to form
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var users = _accountService.GetAllUsers();
            IEnumerable<ApplicationUser> filteredUsers;
            //Check whether the Categories should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                //var nameFilter = Convert.ToString(Request["sSearch_1"]);
                //var addressFilter = Convert.ToString(Request["sSearch_2"]);
                //var townFilter = Convert.ToString(Request["sSearch_3"]);

                //Optionally check whether the columns are searchable at all 
                //var isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                //var isAddressSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                //var isTownSearchable = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredUsers = _accountService.GetAllUsers().Where(aa => aa.UserName.ToLower().Contains(param.sSearch.ToLower()));
                //   .Where(c => isNameSearchable && c.CategoriesName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredUsers = users;
            }

            var isUsernameSort = Convert.ToBoolean(Request["bSortable_1"]);
            //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ApplicationUser, string> orderingFunction = (c => c.UserName);

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredUsers = filteredUsers.OrderBy(orderingFunction);
            else
                filteredUsers = filteredUsers.OrderByDescending(orderingFunction);

            var displayedUsers = filteredUsers.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedUsers select new[] { c.Id, c.UserName, c.Email, c.PhoneNumber };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = users.Count(),
                iTotalDisplayRecords = filteredUsers.Count(),
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }
        #endregion
        private string getIP()
        {
            string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            return ip;
        }
        private void getClaimsByWallet(int WalletId)
        {
            using (WebClient webClient = new WebClient())
            {

                try
                {
                    var wallet = _walletServices.GetWallet(WalletId);
                    string baseAddress = String.Format("https://faucet.raiblockscommunity.net/paylist.php?acc={0}&json=1", wallet.Code);
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
                    webClient.Headers.Add(HttpRequestHeader.Accept, "application/json, text/javascript, */*; q=0.01");
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
                    webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    string baseSiteString = webClient.DownloadString(baseAddress);
                    if (baseSiteString.Contains("ask-till-now") && baseSiteString.Contains(wallet.Code))
                    {
                        string sourceClaim = Regex.Match(baseSiteString, wallet.Code + "(.*?),\"ask-till-now").Value;
                        string sourceAskTillNow = Regex.Match(baseSiteString, "ask-till-now\":(.*?),\"expected-pay").Value;
                        int claim = int.Parse(sourceClaim.Replace(wallet.Code + "\",\"pending\":", "").Replace(",\"ask-till-now", ""));
                        int asktillnow = int.Parse(sourceAskTillNow.Replace("ask-till-now\":", "").Replace(",\"expected-pay", ""));
                        //thuc thi store: 1= co du lieu
                        _walletServices.UpdateWalletStore(wallet.Id, claim, asktillnow);

                    }
                    else if (baseSiteString.Contains("\"pending\":[]"))
                    {
                        //thuc thi store: 0= ko co du lieu
                        _walletServices.UpdateWalletStore(wallet.Id, 0, 0);
                    }
                    webClient.Dispose();
                }
                catch (Exception ex)
                {
                }
            }
        }
        public string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Vui lòng click vào đây để xác nhận: " + callbackUrl);

            return callbackUrl;
        }
        private string SendMail(string address, NameValueCollection values)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] apiResponse = client.UploadValues(address, values);
                    return Encoding.UTF8.GetString(apiResponse);

                }
                catch (Exception ex)
                {
                    return "Exception caught: " + ex.Message + "\n" + ex.StackTrace;
                }
            }
        }
    }
}