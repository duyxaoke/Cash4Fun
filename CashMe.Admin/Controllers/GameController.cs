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
using System.Collections;
using System.Data;
using System.Transactions;
using CashMe.Service.Role;

namespace CashMe.Admin.Controllers
{
    [Authorize]
    public class GameController : ServiceController
    {
        AccountServices _accountServices = new AccountServices();
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
        BetServices _betServices = new BetServices();
        string ResultRaceId = string.Empty;

        // GET: Game
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThapNhi()
        {
            var lsBet = _betServices.GetAllBetbyUser(getCurrentUserId());
            return View(lsBet);
        }
        public JsonResult getInfoUser()
        {
            try
            {
                string UserId = getCurrentUserId();
                var userInfo = _userInfoServices.UserInfoGameView(UserId);
                return Json(new { Status = 1, TotalCoin = userInfo.TotalCoin, RealCoin = userInfo.RealCoin, Amount = userInfo.Amount, TotalBet = userInfo.TotalBet }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateResultRace()
        {
            try
            {
               
                string resultRaceId = Guid.NewGuid().ToString();
                _resultRaceServices.InserResultRaceStore(resultRaceId);
                Session["ResultRaceId"] = resultRaceId;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AcceptBet(int animal1, int animal2, int coin1 = 0, int coin2 = 0)
        {
            try
            {
                string UserId = getCurrentUserId();
                ResultRaceId = Session["ResultRaceId"].ToString();
                int result = _betServices.CreateBetStore(UserId, ResultRaceId, animal1, animal2, coin1, coin2);
                switch(result)
                {
                    case 0: return Json(new { Status = 0, Message = "Có lỗi, vui lòng tải lại trang!" }, JsonRequestBehavior.AllowGet);
                    case 1: return Json(new { Status = 1, Message = "Đặt cượt thành công.!" }, JsonRequestBehavior.AllowGet);
                    case 2: return Json(new { Status = 0, Message = "Bạn không đủ coin để đặt cược.!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Status = 0, Message = "Có lỗi, vui lòng tải lại trang!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = 0, Message = "Có lỗi, vui lòng tải lại trang!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult getResultRace()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    string UserId = getCurrentUserId();
                    var lsTOP = _animalMasterServices.GetAllAnimalMaster().OrderBy(c => Guid.NewGuid()).ToArray();
                    ResultRaceId = Session["ResultRaceId"].ToString();
                    //update resultRace using store
                    _resultRaceServices.UpdateResultRaceStore(ResultRaceId, lsTOP[0].Id, lsTOP[1].Id, lsTOP[2].Id, lsTOP[3].Id, lsTOP[4].Id, lsTOP[5].Id, lsTOP[6].Id, lsTOP[7].Id, lsTOP[8].Id, lsTOP[9].Id, lsTOP[10].Id, lsTOP[11].Id);

                    //tinh toan tien cho member
                    string lsResult = string.Join(",", lsTOP.Select(c => c.Id).ToArray());
                    _resultRaceServices.Final_ResultRace(lsResult, UserId, ResultRaceId);

                    //get info
                    var userInfo = _userInfoServices.UserInfoGameView(UserId);
                    ts.Complete();

                    return Json(new { lsTOP = lsTOP, Status = 1, TotalCoin = userInfo.TotalCoin, RealCoin = userInfo.RealCoin, Amount = userInfo.Amount, TotalBet = userInfo.TotalBet }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception)
                {
                    return Json(new { Status = 0 }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                }
            }

            
        }
        public JsonResult getResultBet()
        {
            ResultRaceId = Session["ResultRaceId"].ToString();
            var lsBet = _betServices.GetBetbyUser(ResultRaceId, getCurrentUserId());
            return Json(lsBet, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RandomBet()
        {
            CreateResultRace();
            var lsTOP = _animalMasterServices.GetAllAnimalMaster().OrderBy(x => Guid.NewGuid()).Take(6);
            string dogCsv = string.Join(",", lsTOP.Select(c=>c.Id).ToArray());
            return Json(lsTOP, JsonRequestBehavior.AllowGet);
        }

    }
}