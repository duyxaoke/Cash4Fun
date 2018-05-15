
using CashMe.Admin.Models;
using CashMe.Service;
using CashMe.Service.Models;
using CashMe.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    public class ManagerPaymentController : Controller
    {
        #region Contractor
        private IAccountServices _AccountServices;
        private IClaimsServices _claimsServices;
        private IPaymentServices _paymentServices;
        private IWalletServices _walletServices;

        public ManagerPaymentController(IAccountServices AccountServices, IClaimsServices ClaimsServices,
            IPaymentServices PaymentServices, IWalletServices WalletServices)
        {
            this._AccountServices = AccountServices;
            this._claimsServices = ClaimsServices;
            this._paymentServices = PaymentServices;
            this._walletServices = WalletServices;
        }
        #endregion


        // GET: ManagerPayment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult updatePayment(int id, string content)
        {
            try
            {
                if (id != 0)
                {
                    var update = _paymentServices.GetPayment(id);
                    update.Content = content;
                    update.Status = 1;
                    _paymentServices.UpdatePayment(update);
                    _paymentServices.Save();
                }
                return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getPayment(int id)
        {
            return Json(_paymentServices.GetPayment(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var allPayment = _paymentServices.PaymentView();
            IEnumerable<PaymentModel> filteredPayment;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                var nameFilter = Convert.ToString(Request["sSearch_1"]);
                var addressFilter = Convert.ToString(Request["sSearch_2"]);
                var townFilter = Convert.ToString(Request["sSearch_3"]);

                //Optionally check whether the columns are searchable at all 
                var isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var isAddressSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isTownSearchable = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredPayment = _paymentServices.PaymentView()
                   .Where(c => isNameSearchable && c.Username.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isAddressSearchable && c.Email.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredPayment = allPayment;
            }

            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PaymentModel, string> orderingFunction = (c => sortColumnIndex == 1 && isNameSortable ? c.Username :
                                                           sortColumnIndex == 2 && isTownSortable ? c.Email :
                                                           sortColumnIndex == 3 && isAddressSortable ? c.CountImage.ToString() :
                                                           sortColumnIndex == 4 && isTownSortable ? c.Amount.ToString() :
                                                           sortColumnIndex == 5 && isTownSortable ? c.Content.ToString() :
                                                           sortColumnIndex == 6 && isTownSortable ? c.Status.ToString() :
                                                           sortColumnIndex == 7 && isTownSortable ? c.CreateDate.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredPayment = filteredPayment.OrderBy(orderingFunction);
            else
                filteredPayment = filteredPayment.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredPayment.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id),
                c.Username, c.Email, c.CountImage.ToString(), c.Amount.ToString(),
                c.Content.ToString(), c.Status.ToString(),
                c.CreateDate.Value.ToString("dd/MM/yyyy hh:mm:ss") };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allPayment.Count(),
                iTotalDisplayRecords = filteredPayment.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

    }
}