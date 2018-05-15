using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashMe.Service;
using CashMe.Core.Data;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CashMe.Shared.Common;
using System.Net;
using Newtonsoft.Json;
using CashMe.Admin.Models;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CaptchaMvc.HtmlHelpers;
using System.Net.NetworkInformation;
using RazorEngine.Templating;
using System.Management;
using System.Collections.Specialized;
using System.Net.Mail;

namespace CashMe.Admin.Controllers
{
    [Authorize]
    public class HomeController : ServiceController
    {

        private ApplicationUserManager _userManager;
        private AccountServices _AccountServices = new AccountServices();
        private ClaimsServices _claimsServices = new ClaimsServices();
        private PaymentServices _paymentServices = new PaymentServices();
        private WalletServices _walletServices = new WalletServices();
        private ConfigServices _configServices = new ConfigServices();
        private UserRefServices _userRefServices = new UserRefServices();
        private UserInfoServices _userInfoServices = new UserInfoServices();
        private TargetServices _targetServices = new TargetServices();
        private ReportTOPServices _reportTOPServices = new ReportTOPServices();

        public HomeController()
        {
        }
        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        [AllowAnonymous]
        public ActionResult Ngu()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult T()
        {
            string smtpAddress = "smtp.mail.yahoo.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "cash4fun@yahoo.com";
            string password = "12345_abc";
            string emailTo = "duytran1402@gmail.com";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = "test";
                mail.Body = "hhh";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            return View();
        }
        public ActionResult TemplateEmail()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Index(string r = null)
        {
            ViewBag.GiaCoin = _configServices.GetConfig().GiaCoin;
            Session["ref"] = r;
            return View();
        }
        public ActionResult ConfirmEmail()
        {
            string Username = System.Web.HttpContext.Current.User.Identity.GetUserName();
            if (_AccountServices.GetEmailConfirm(Username))
            {
                return RedirectToAction("Dashboard");
            }
            ApplicationUser user = UserManager.FindByName(Username);
            ViewBag.Mail = user.Email;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(FormCollection form)
        {
            var User = getCurrentUsers();
            ViewBag.Mail = User.Email;
            if (_AccountServices.GetUserbyEmail(form["Email"].ToString()).Count() > 1)
            {
                ViewBag.Error = "Email đã tồn tại trong hệ thống, vui lòng chọn mail khác.";
                return View();
            }

            var config = _configServices.GetConfig();
            config.MailSendToday += 1;
            _configServices.UpdateConfig(config);

            string callbackUrl = await SendEmailConfirmationTokenAsync(User.Id, "Xác thực tài khoản - Cash4Fun");
            _AccountServices.UpdateEmail(User.UserName, form["Email"].ToString());
            return RedirectToAction("Info", "Account");
        }

        public ActionResult Dashboard()
        {
            //clear 
            var userInfoView = getCurrentUsers();


            //send mail
            //if (!_AccountServices.GetEmailConfirm(userInfoView.UserName))
            //{
            //    if (_configServices.GetConfig().MailSendToday < 400)
            //        return RedirectToAction("ConfirmEmail");
            //}

            //var wallet = new Wallet();

            //if (_userInfoServices.CheckWalletUserInfo(userInfoView.Id).WalletId == 0)
            //{
            //    wallet = getWallet();
            //    getClaimsByWallet(wallet.Id);
            //}
            //else
            //{
            //    wallet = getWallet();
            //}
            //ViewBag.Wallet = wallet;
            ViewBag.RealValue = userInfoView.Amount;
            ViewBag.CountImage = userInfoView.CountImage;
            ViewBag.TotalRef = userInfoView.Ref;
            ViewBag.UserId = userInfoView.Id;
            ViewBag.Target = userInfoView.Target;
            ViewBag.StatusBonus = userInfoView.StatusBonus;

            ViewBag.lsTargetMaster = getAllTargetMaster();
            ViewBag.GiaCoin = _configServices.GetConfig().GiaCoin;
            return View();
        }
        public ActionResult Payment()
        {
            //string Username = System.Web.HttpContext.Current.User.Identity.GetUserName();
            ////send mail
            //if (!_AccountServices.GetEmailConfirm(Username))
            //{
            //    if (_configServices.GetConfig().MailSendToday < 400)
            //        return RedirectToAction("ConfirmEmail");
            //}

            var Amount = getAmount();
            ViewBag.RealValue = Amount;
            ViewBag.TotalValue = Amount;

            return View();
        }
        public ActionResult History()
        {
            return View(getPayment());
        }
        [AllowAnonymous]
        public ActionResult Challenge()
        {
            ViewBag.lsReportTOP_Week = _reportTOPServices.ReportTOP_Week();
            ViewBag.lsReportTOP_Month = _reportTOPServices.ReportTOP_Month();
            return View();//_userInfoServices.UserView()
        }
        public ActionResult ListRefs()
        {
            string UserId = getCurrentUserId();
            var lsUserRefView = _userRefServices.GetUserRefView(UserId);
            return View(lsUserRefView);
        }
        public ActionResult Chat()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentCard(FormCollection f)
        {
            string Username = System.Web.HttpContext.Current.User.Identity.GetUserName();

            //save MAC + ComputerName
            //var MAC = GetMACAddress();

            //string ComputerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;

            //int ckOption = int.Parse(f.Get("options"));

            //neu xac thuc bang dia chi mac
            //if (ckOption == 1)
            //{
            //    var uInfo = _userInfoServices.GetUserInfobyUser(getCurrentUserId());
            //    if(uInfo.MAC != MAC)
            //    {
            //        ViewBag.RealValue = getAmount();
            //        ViewBag.TotalValue = getAmount();
            //        ViewBag.Error = "Địa chỉ MAC không đúng, hãy sử dụng máy tính của mình để mua thẻ nhé.";
            //        return View("Payment");

            //    }
            //}

            if (!ModelState.IsValid)
            {
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            try
            {
                if (!this.IsCaptchaValid("Validate your captcha"))
                {
                    ViewBag.Error = "Sai captcha";
                }
                else
                {
                    int Amount = int.Parse(f["card_amount"]);

                    string UserId = getCurrentUserId();
                    var userInfox = _userInfoServices.GetUserInfobyUser(UserId);
                    double AmountUser = userInfox.Amount;
                    if (AmountUser >= Amount && Amount <= 100000 && Amount >= 10000)
                    {
                        if (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 19)
                        {
                            ViewBag.Error = "Thời gian mở thanh toán từ 8h - 20h hàng ngày. Bạn quay lại sau nhé ^^";
                        }
                        else
                        {
                            string card_type = f["card_type"];
                            string ProductCode = string.Empty;
                            string nameCard = string.Empty;
                            switch (card_type)
                            {
                                case "VTT": ProductCode = "500"; nameCard = "VIETTEL"; break;
                                case "VMS": ProductCode = "501"; nameCard = "MOBIFONE"; break;
                                case "VNP": ProductCode = "502"; nameCard = "VINAPHONE"; break;
                                case "VTC": ProductCode = "300"; nameCard = "VTC"; break;
                                case "Gate": ProductCode = "301"; nameCard = "Gate"; break;
                                case "Garena": ProductCode = "302"; nameCard = "Garena"; break;
                                case "Zing": ProductCode = "303"; nameCard = "Zing"; break;
                                case "OnCash": ProductCode = "305"; nameCard = "OnCash"; break;
                                case "Megacard": ProductCode = "306"; nameCard = "Megacard"; break;
                            }
                            //buy card
                            int productCode = int.Parse(ProductCode);

                            //cardt game
                            string input = string.Empty;
                            if (productCode < 500)
                            {
                                input = "{\"ProductCode\":\"" + ProductCode + "\",\"RefNumber\":\"" + Username + "-" + Guid.NewGuid().ToString() + "\",\"CustIP\":\"127.0.0.1\",\"CardPrice\":\"" + Amount + "\",\"CardQuantity\":\"1\"}";
                            }
                            else
                            {
                                input = "{\"ProductCode\":\"" + ProductCode + "\",\"RefNumber\":\"" + Username + "-" + Guid.NewGuid().ToString() + "\",\"Telco\":\"" + card_type + "\",\"CustMobile\":\"\",\"CustIP\":\"127.0.0.1\",\"CardPrice\":\"" + Amount + "\",\"CardQuantity\":\"1\"}";

                            }

                            var key = "wYVRH8oEVci8ItSz5mWSJ9MT";
                            var md5key = "aJQCunMndT96lDyZUP";

                            var encData = Encrypt(key, input);

                            var fnc = "buyPrepaidCards";
                            var ver = "1.0";
                            var agentID = "20170707160144";
                            var accID = "595f4df8e4b01f4ae891c88a";
                            var checksum = MD5PHP(fnc + ver + agentID + accID + encData + md5key);

                            var obj = new
                            {
                                Fnc = fnc,
                                Ver = ver,
                                AgentID = agentID,
                                AccID = accID,
                                EncData = encData,
                                Checksum = checksum
                            };

                            var myParameters = JsonConvert.SerializeObject(obj);

                            var content = Post(myParameters);
                            var responseCard = JsonConvert.DeserializeObject<Result>(content);
                            if (responseCard.RespCode == "00")
                            {
                                var body = Decrypt11(key, responseCard.EncData);
                                var root = JsonConvert.DeserializeObject<RootCard>(body);
                                if (root.CardInfo.Count > 0)
                                {
                                    var user = getCurrentUsers();


                                    var userInfo = _userInfoServices.GetUserInfobyUser(UserId);
                                    userInfo.Amount = userInfo.Amount - Amount;
                                    _userInfoServices.UpdateUserInfo(userInfo);

                                    string success = "Thành công!<br/>";
                                    success += "Loại thẻ: " + nameCard + "<br/>";
                                    success += "Seri: " + root.CardInfo[0].card_serial + "<br/>";
                                    success += "Mã thẻ: " + root.CardInfo[0].card_code + "<br/>";
                                    success += "Ngày hết hạn: " + root.CardInfo[0].expiration_date + "<br/>";



                                    Payment pay = new Payment();
                                    pay.UserId = UserId;
                                    pay.Amount = Amount;
                                    pay.Content = success;
                                    pay.Status = 1; //ok
                                    pay.CreateDate = DateTime.Now;
                                    pay.MAC = string.Empty;
                                    pay.ComputerName = string.Empty;
                                    pay.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
                                    pay.TypePayment = 1;
                                    _paymentServices.InserPayment(pay);

                                    ViewBag.RealValue = AmountUser;
                                    ViewBag.TotalValue = AmountUser;

                                    //if (ckOption == 1)
                                    //{
                                    //    ViewBag.Success = success;

                                    //}
                                    //else
                                    //{
                                    //success = "Loại thẻ: " + nameCard + " - Seri: " + root.CardInfo[0].card_serial + " - Mã thẻ: " + root.CardInfo[0].card_code;
                                    //NameValueCollection values = new NameValueCollection();
                                    //values.Add("apikey", "7751d86b-8f7d-41a9-a900-b5c63b88e549");
                                    //values.Add("from", "support@cash4fun.net");
                                    //values.Add("fromName", "Cash4Fun Solution");
                                    //values.Add("to", user.Email);
                                    //values.Add("subject", "93658222D" + pay.Id + " - Cash4Fun thanh toán thẻ " + nameCard);
                                    //values.Add("bodyText", success);
                                    //values.Add("bodyHtml", success);
                                    ////values.Add("isTransactional", true);

                                    //string address = "https://api.elasticemail.com/v2/email/send";

                                    //SendMail(address, values);
                                    //string mail = user.Email;
                                    //mail = mail.Substring(0, 3) + "***" + mail.Substring(5);
                                    //ViewBag.Success = "Mã thẻ đã được gởi về mail " + mail;
                                    ViewBag.Success = success;
                                    //}
                                }
                                else
                                {
                                    ViewBag.Error = "Có lỗi khi mua mã thẻ.!";
                                }
                            }
                            else
                            {
                                ViewBag.Error = "Có lỗi khi mua thẻ, vui lòng liên hệ admin!";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Success = null;
                        ViewBag.Error = "Không đủ số dư";
                    }

                }
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            catch (Exception ex)
            {
                ViewBag.Success = null;
                ViewBag.Error = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return View("Payment");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PaymentBank(FormCollection f)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            try
            {
                int Amount = int.Parse(f["bank_tienrut"]);
                if (getAmount() >= Amount)
                {
                    var user = getCurrentUsers();

                    string UserId = getCurrentUserId();
                    Payment pay = new Payment();
                    pay.UserId = UserId;
                    pay.Amount = Amount;
                    string Content = "";
                    Content += "Ngân hàng: " + f.Get("bank_nganhang") + "</br>";
                    Content += "Tỉnh/Thành phố: " + f.Get("bank_thanhpho") + "</br>";
                    Content += "Chi nhánh: " + f["bank_chinhanh"] + "</br>";
                    Content += "Chủ TK: " + f["bank_chutaikhoan"] + "</br>";
                    Content += "STK: " + f["bank_stk"] + "</br>";
                    pay.Content = Content;
                    pay.TypePayment = 1;
                    pay.Status = 0; //pending
                    pay.CreateDate = DateTime.Now;
                    _paymentServices.InserPayment(pay);

                    var userInfo = _userInfoServices.GetUserInfobyUser(UserId);
                    userInfo.Amount = userInfo.Amount - Amount;
                    _userInfoServices.UpdateUserInfo(userInfo);

                    ViewBag.Success = "Gửi yêu cầu thanh toán thành công, vui lòng vào Lịch sử thanh toán để kiểm tra";
                    ViewBag.Error = null;

                }
                else
                {
                    ViewBag.Success = null;
                    ViewBag.Error = "Không đủ số dư";
                }
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            catch (Exception ex)
            {
                ViewBag.Success = null;
                ViewBag.Error = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return View("Payment");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PaymentMomo(FormCollection f)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            try
            {
                int Amount = int.Parse(f["momo_tienrut"]);
                if (getAmount() >= Amount)
                {
                    var user = getCurrentUsers();

                    string UserId = getCurrentUserId();
                    Payment pay = new Payment();
                    pay.UserId = UserId;
                    pay.Amount = Amount;
                    string Content = "Thanh toán qua Momo </br>";
                    Content += "Họ tên: " + f.Get("momo_hoten") + "</br>";
                    Content += "SĐT: " + f.Get("momo_dienthoai") + "</br>";
                    pay.Content = Content;
                    pay.TypePayment = 1;
                    pay.Status = 0; //pending
                    pay.CreateDate = DateTime.Now;
                    _paymentServices.InserPayment(pay);

                    var userInfo = _userInfoServices.GetUserInfobyUser(UserId);
                    userInfo.Amount = userInfo.Amount - Amount;
                    _userInfoServices.UpdateUserInfo(userInfo);

                    ViewBag.Success = "Gửi yêu cầu thanh toán thành công, vui lòng vào Lịch sử thanh toán để kiểm tra";
                    ViewBag.Error = null;

                }
                else
                {
                    ViewBag.Success = null;
                    ViewBag.Error = "Không đủ số dư";
                }
                ViewBag.RealValue = getAmount();
                ViewBag.TotalValue = getAmount();
                return View("Payment");
            }
            catch (Exception ex)
            {
                ViewBag.Success = null;
                ViewBag.Error = "Có lỗi xảy ra, vui lòng thử lại sau!";
                return View("Payment");
            }
        }

        //clear truoc khi login nhan vi moi
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
        [AllowAnonymous]
        public JsonResult getLink()
        {
            ResultWallet result = new ResultWallet();

            try
            {
                CookieContainer cookies = new CookieContainer();
                SmsWebClient client = new SmsWebClient(cookies);
                string baseSiteString = client.DownloadString("https://faucet.raiblockscommunity.net/form.php");
                if (baseSiteString.Contains("/hcaptcha/api/verify"))
                {
                    string sourceClaim = "OK";
                    return Json(sourceClaim, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getClaims1()
        {
            ResultWallet result = new ResultWallet();

            try
            {
                var wallet = getWallet();
                string UserId = getCurrentUserId();
                CookieContainer cookies = new CookieContainer();
                SmsWebClient client = new SmsWebClient(cookies);
                string baseSiteString = client.DownloadString(String.Format("https://faucet.raiblockscommunity.net/paylist.php?acc={0}&json=1", wallet.Code));
                if (baseSiteString.Contains("ask-till-now") && baseSiteString.Contains(wallet.Code))
                {
                    string sourceClaim = Regex.Match(baseSiteString, wallet.Code + "(.*?),\"ask-till-now").Value;
                    string sourceAskTillNow = Regex.Match(baseSiteString, "ask-till-now\":(.*?),\"expected-pay").Value;
                    int claim = int.Parse(sourceClaim.Replace(wallet.Code + "\",\"pending\":", "").Replace(",\"ask-till-now", ""));
                    int asktillnow = int.Parse(sourceAskTillNow.Replace("ask-till-now\":", "").Replace(",\"expected-pay", ""));

                    //update by store
                    _claimsServices.UpdateClaims_1Minute(UserId, wallet.Code, asktillnow, claim);
                    result.Coin = getCurrentUsers().CountImage;
                    result.RealValue = getAmount();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getClaims()
        {
            ResultWallet result = new ResultWallet();

            using (WebClient webClient = new WebClient())
            {

                try
                {
                    var wallet = getWallet();
                    string UserId = getCurrentUserId();
                    string baseAddress = String.Format("https://faucet.raiblockscommunity.net/paylist.php?acc={0}&json=1", wallet.Code);
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    webClient.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0");
                    //webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    string baseSiteString = webClient.DownloadString(baseAddress);
                    if (baseSiteString.Contains("ask-till-now") && baseSiteString.Contains(wallet.Code))
                    {
                        string sourceClaim = Regex.Match(baseSiteString, wallet.Code + "(.*?),\"ask-till-now").Value;
                        string sourceAskTillNow = Regex.Match(baseSiteString, "ask-till-now\":(.*?),\"expected-pay").Value;
                        int claim = int.Parse(sourceClaim.Replace(wallet.Code + "\",\"pending\":", "").Replace(",\"ask-till-now", ""));
                        int asktillnow = int.Parse(sourceAskTillNow.Replace("ask-till-now\":", "").Replace(",\"expected-pay", ""));
                        webClient.Dispose();

                        //update by store
                        _claimsServices.UpdateClaims_1Minute(UserId, wallet.Code, asktillnow, claim);
                        result.Coin = getCurrentUsers().CountImage;
                        result.RealValue = getAmount();
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #region Buycard
        public static string Post(string myParameters)
        {



            string URI = "https://api.alego.vn/agent_api/";

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
                try
                {
                    string HtmlResult = wc.UploadString(URI, "Post", myParameters);

                    var result = JsonConvert.DeserializeObject(HtmlResult);

                    return HtmlResult;
                }
                catch (WebException ex)
                {
                    var statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                    var des = ((HttpWebResponse)ex.Response).StatusDescription;
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    return resp;
                }
            }
        }

        public static string MD5PHP(string input)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(input);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedString;
        }

        public static string Encrypt11(string key, string data)
        {
            data = data.Trim();
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Padding = PaddingMode.PKCS7;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
            CryptoStreamMode.Write);
            encStream.Write(Encoding.ASCII.GetBytes(data), 0,
            Encoding.ASCII.GetByteCount(data));
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            ms.Close();
            encStream.Close();
            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }

        public static string Encrypt(string key, string data)
        {
            data = data.Trim();
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Padding = PaddingMode.PKCS7;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
            CryptoStreamMode.Write);
            encStream.Write(Encoding.ASCII.GetBytes(data), 0,
            Encoding.ASCII.GetByteCount(data));
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            ms.Close();
            encStream.Close();
            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }

        public static string Decrypt11(string key, string dataen)
        {
            byte[] toEncryptArray = Convert.FromBase64String(dataen);
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Padding = PaddingMode.PKCS7;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            ICryptoTransform ict = tripdes.CreateDecryptor();
            byte[] resultArray = ict.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tripdes.Clear();
            return Encoding.ASCII.GetString(resultArray);
        }

        #endregion
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
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
    #region Buycard
    public class Result
    {
        public string Fnc { get; set; }
        public string Ver { get; set; }
        public string AgentID { get; set; }
        public string AccID { get; set; }
        public string RespCode { get; set; }
        public string EncData { get; set; }
        public string Description { get; set; }
        public string Checksum { get; set; }
    }
    public class CardInfo
    {
        public string card_code { get; set; }
        public string card_serial { get; set; }
        public string expiration_date { get; set; }
    }

    public class RootCard
    {
        public string ProductCode { get; set; }
        public string RefNumber { get; set; }
        public string TransID { get; set; }
        public string TransDate { get; set; }
        public string ResType { get; set; }
        public int CardQuantity { get; set; }
        public List<CardInfo> CardInfo { get; set; }
    }
    #endregion
}
