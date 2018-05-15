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
using OfficeOpenXml;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using CashMe.Data.DAL;
using System.Transactions;
using CashMe.Service.Role;
using CashMe.Admin.Models;
using HtmlAgilityPack;
using CashMe.Service.Models;

namespace CashMe.Admin.Controllers
{
    public class ServiceController : Controller
    {
        AccountServices _AccountServices = new AccountServices();
        ClaimsServices _claimsServices = new ClaimsServices();
        PaymentServices _paymentServices = new PaymentServices();
        WalletServices _walletServices = new WalletServices();
        UserInfoServices _userInfoServices = new UserInfoServices();
        UserRefServices _userRefServices = new UserRefServices();
        RoleServices _roleServices = new RoleServices();
        ConfigServices _configServices = new ConfigServices();
        TargetServices _targetServices = new TargetServices();
        TargetMasterServices _targetMasterServices = new TargetMasterServices();
        AnimalMasterServices _animalMasterServices = new AnimalMasterServices();
        ResultRaceServices _resultRaceServices = new ResultRaceServices();
        // GET: Service
        public JsonResult MainView()
        {
            return Json(_walletServices.getMainView(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Test()
        {
            return Json(_configServices.GetAllConfig(), JsonRequestBehavior.AllowGet);
        }
        public string GetWalletString()
        {
            string result = string.Empty;
            var wallet = _walletServices.GetAllWallet().Where(c=>c.Status == 0);
            foreach (var item in wallet)
            {
                result += item.Code + "|";
            }
            return result;
        }

        //public JsonResult AddClaimApp(int Coin, string UserId)
        //{
        //    try
        //    {
        //        _userInfoServices.AddClaimApp(Coin, UserId);
        //        return Json(new { Status = 1, Message = "Success"/*, Info = _userInfoServices.getUserInfoByStore(UserId)*/ }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Status = 1, Message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult updateTotalCoinWallet(string code, int claim, int askstillnow)
        {
            var wallet = _walletServices.GetWalletbyCode(code);

            
                wallet.Claim = claim;
                wallet.AskStillNow = askstillnow;
                wallet.UpdateDate = DateTime.Now;
                _walletServices.UpdateWallet(wallet);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriceCoin()
        {
            var client = new WebClient();
            var content = client.DownloadString("https://api.coinmarketcap.com/v1/ticker/raiblocks");
            var json = JsonConvert.DeserializeObject<List<PriceRaiblock>>(content);
            if (json != null)
            {
                float price_usd =  float.Parse(json[0].price_usd);

                var rai = client.DownloadString("https://faucet.raiblockscommunity.net/paylist.php");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(rai);

                HtmlNode table = doc.DocumentNode.SelectSingleNode("//*[@id='page-top']/section/div/div[4]/table");
                if(table == null)
                {
                    table = doc.DocumentNode.SelectSingleNode("//*[@id='page-top']/section/div/div[3]/table");
                }
                float claim = float.Parse(table.SelectNodes(".//tr[last()]/td")[2].InnerText);
                float mrai = float.Parse(table.SelectNodes(".//tr[last()]/td")[4].InnerText);
                var price = 1000 * mrai / claim * price_usd * 22700;
                return Json(price, JsonRequestBehavior.AllowGet);

            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #region user Funtion
        public string getCurrentUserId()
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            return UserId;
        }
        public UserViewModel getCurrentUsers()
        {
            string UserId = getCurrentUserId();
            var userInfo = _userInfoServices.getUserInfoByStore(UserId).FirstOrDefault();
            return userInfo;
        }
        public int getTargetByUser()
        {
            return getCurrentUsers().Target;

        }
        public Wallet getWallet()
        {
            string UserId = getCurrentUserId();
            var wallet = _walletServices.getWalletStore(UserId).FirstOrDefault();
            return wallet;
        }
        public Wallet getWalletApp(string UserId)
        {
            var wallet = _walletServices.getWalletStore(UserId).FirstOrDefault();
            return wallet;
        }
        public double getAmount()
        {
            return getCurrentUsers().Amount;
        }
        public int getCountImage()
        {
            return getCurrentUsers().CountImage;
        }
        public int getRef()
        {
            return getCurrentUsers().Ref;
        }
        public IEnumerable<Payment> getPayment()
        {
            string UserId = getCurrentUserId();
            var pay = _paymentServices.GetPaymentbyUser(UserId);
            return pay;
        }
        #endregion
        public ActionResult Upload()
        {
            return View();
        }
        #region Upload
        [HttpPost]
        public ActionResult UploadUser(FormCollection formCollection)
        {
            if (Request != null)
            {
                CashMeContext _context = new CashMeContext();
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {

                            string email = workSheet.Cells[rowIterator, 1].Value.ToString();
                            string password = workSheet.Cells[rowIterator, 2].Value.ToString();
                            string SecurityStamp = workSheet.Cells[rowIterator, 3].Value.ToString();
                            string IP = workSheet.Cells[rowIterator, 4].Value.ToString();
                            string WalletCode = workSheet.Cells[rowIterator, 5].Value.ToString();
                            int Ref = int.Parse(workSheet.Cells[rowIterator, 6].Value.ToString());

                            string username = Regex.Split(email, "@")[0];

                            var user = new ApplicationUser();
                            user.Email = email;
                            user.PasswordHash = password;
                            user.SecurityStamp = SecurityStamp;
                            user.PhoneNumberConfirmed = false;
                            user.TwoFactorEnabled = false;
                            user.LockoutEnabled = true;
                            user.AccessFailedCount = 1;
                            user.UserName = username;

                            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
                            var store = new UserStore<ApplicationUser>(_context);
                            var manager = new UserManager<ApplicationUser>(store);
                            var existUser = userManager.Users.FirstOrDefault(aa => aa.Id == user.Id);
                            if (existUser != null)
                            {
                                manager.Update(user);
                                var ctx = store.Context;
                                ctx.SaveChanges();
                                var lstRole = new string[] { DefaultData.RoleUser };
                                var allRole = userManager.GetRoles(existUser.Id).ToArray();
                                if (allRole != null && allRole.Any())
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        userManager.RemoveFromRoles(existUser.Id, allRole);
                                        if (lstRole.Length > 0)
                                        {
                                            var roles = _roleServices.GetAllRoles().Where(aa => lstRole.Contains(aa.Id));
                                            var rolesAdd = roles.Select(aa => aa.Name).ToArray();
                                            userManager.AddToRoles(existUser.Id, rolesAdd);
                                        }
                                        scope.Complete();
                                    }
                                }
                            }
                            else
                            {
                                manager.Create(user);
                                var ctx = store.Context;

                                userManager.AddToRole(user.Id, DefaultData.RoleUser);
                                ctx.SaveChanges();
                            }
                            var wallet = _walletServices.GetWalletbyCode(WalletCode);
                            var uInfo = new UserInfo();
                            uInfo.UserId = user.Id;
                            uInfo.WalletId = wallet.Id;
                            uInfo.Amount = (wallet.Claim * 25) + (Ref * 5);
                            uInfo.IP = IP;
                            uInfo.CreateDate = DateTime.Now;
                            uInfo.LastLoginDate = DateTime.Now;
                            _userInfoServices.InserUserInfo(uInfo);
                            var claim = new Claims();
                            claim.UserId = user.Id;
                            claim.CountImage = wallet.Claim;
                            _claimsServices.InserClaims(claim);
                        }
                    }
                }
            }
            return View("Upload");
        }

        [HttpPost]
        public ActionResult UploadWallet(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {

                            string code = workSheet.Cells[rowIterator, 1].Value.ToString();
                            if (code != null)
                            {
                                var updateWalllet = _walletServices.GetWalletbyCode(code);
                                if (updateWalllet == null)
                                {
                                    var wallet = new Wallet();
                                    wallet.Code = code;
                                    wallet.Status = 0;
                                    wallet.Claim = 0;
                                    wallet.UpdateDate = DateTime.Now;
                                    _walletServices.InserWallet(wallet);

                                }

                            }


                        }
                    }
                }
            }
            return View("Upload");
        }
        #endregion
        
        public JsonResult UpdateConfig(float price)
        {
            var config = _configServices.GetConfig();
            config.GiaCoin = price;
            _configServices.UpdateConfig(config);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Offline(bool x)
        {
            var config = _configServices.GetConfig();
            config.IsOnline  = x;
            _configServices.UpdateConfig(config);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<TargetMaster> getAllTargetMaster()
        {
            var ls = _targetMasterServices.GetAllTargetMaster();
            return ls;
        }
        public JsonResult recieveBonus(int id)//id targerMaster
        {
            string UserId = getCurrentUserId();
            var target = _targetServices.GetTargetbyUser(UserId);
            var targetMaster = _targetMasterServices.GetTargetMaster(id);
            if(target.CountImage >= targetMaster.Higher)
            {
                if (target.StatusBonus < targetMaster.Id)
                {
                    int coinBonus = 0;
                    int checkSum = 0;
                    var checkList = _targetMasterServices.GetAllTargetMaster().Where(c => c.Id <= targetMaster.Id);
                    if(checkList != null)
                    {
                        foreach (var item in checkList)
                        {
                            coinBonus += item.Bonus;
                            if (target.StatusBonus >= item.Id)
                            {
                                checkSum += item.Bonus;
                            }
                        }
                    }
                    else
                    {
                        coinBonus = targetMaster.Bonus;
                    }
                    //cong coin bonus
                    var claims = _claimsServices.GetClaimsbyUser(UserId);
                    claims.CountImage = claims.CountImage + (coinBonus - checkSum);
                    _claimsServices.UpdateClaims(claims);
                    //update target
                    target.StatusBonus = targetMaster.Id;
                    _targetServices.UpdateTarget(target);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ResetTaget()
        {
            try
            {
                //goi store Reset_Target
                _targetServices.ResetTarget();
                return Json("OK", JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json("NOT OK", JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult ResetWallet()
        {
            var wallet = _walletServices.GetWalletInActiveStore().ToArray();
            for (int item = 0; item < wallet.Count(); item++)
            {
                try
                {
                    var userInfo = _userInfoServices.GetUserInfobyWallet(wallet[item].Id);

                    string UserId = userInfo == null ? string.Empty : userInfo.UserId;

                    TimeSpan minuteWallet = DateTime.Now - wallet[item].UpdateDate;
                    if (minuteWallet.TotalMinutes >= 15)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            string baseAddress = String.Format("https://faucet.raiblockscommunity.net/paylist.php?acc={0}&json=1", wallet[item].Code);
                            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
                            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json, text/javascript, */*; q=0.01");
                            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
                            webClient.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            string baseSiteString = webClient.DownloadString(baseAddress);
                            if (baseSiteString.Contains("ask-till-now") && baseSiteString.Contains(wallet[item].Code))
                            {
                                string sourceClaim = Regex.Match(baseSiteString, wallet[item].Code + "(.*?),\"ask-till-now").Value;
                                string sourceAskTillNow = Regex.Match(baseSiteString, "ask-till-now\":(.*?),\"expected-pay").Value;
                                int claim = int.Parse(sourceClaim.Replace(wallet[item].Code + "\",\"pending\":", "").Replace(",\"ask-till-now", ""));
                                int asktillnow = int.Parse(sourceAskTillNow.Replace("ask-till-now\":", "").Replace(",\"expected-pay", ""));
                                //thuc thi store: 1= co du lieu
                                _claimsServices.UpdateClaims_30Minute(UserId, wallet[item].Id, asktillnow, claim, 1);

                            }
                            else if (baseSiteString.Contains("\"pending\":[]"))
                            {
                                //thuc thi store: 0= ko co du lieu
                                _claimsServices.UpdateClaims_30Minute(UserId, wallet[item].Id, 0, 0, 0);
                            }

                            webClient.Dispose();
                        }
                    }
                }

                catch (Exception ex)
                {
                    continue;
                }

            }

            //update wallet UserInfo, if Wallet Status = 0
            _userInfoServices.UpdateWalletUserInfoByStore();
            return Json("OK", JsonRequestBehavior.AllowGet);

        }


        #region Game
        public JsonResult RandomAnimals()
        {
            Random r = new Random();
            var lsResultRace = new ResultRace();

            var lsAnimalMaster = _animalMasterServices.GetAllAnimalMaster();
            return Json(lsAnimalMaster.OrderBy(n => Guid.NewGuid()), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}