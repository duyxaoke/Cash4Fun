
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ManagerUserController : ServiceController
    {
        AccountServices _AccountServices = new AccountServices();
        ClaimsServices _claimsServices = new ClaimsServices();
        PaymentServices _paymentServices = new PaymentServices();
        WalletServices _walletServices = new WalletServices();


        // GET: ManagerUser
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult DisableUser(int id)
        {
            try
            {
                if (id != 0)
                {
                    var model = _AccountServices.GetUser(id);
                    _AccountServices.DisableUser(model);
                    _AccountServices.Save();
                    return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var allUser = _AccountServices.GetAllUsers();
            IEnumerable<Users> filteredUser;
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

                filteredUser = _AccountServices.GetAllUsers()
                   .Where(c => isNameSearchable && c.FullName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isAddressSearchable && c.Email.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredUser = allUser;
            }

            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Users, string> orderingFunction = (c => sortColumnIndex == 1 && isNameSortable ? c.FullName :
                                                           sortColumnIndex == 2 && isTownSortable ? c.Amount.ToString() :
                                                           sortColumnIndex == 3 && isAddressSortable ? c.Email :
                                                           sortColumnIndex == 4 && isTownSortable ? c.IsOnline.ToString() :
                                                           sortColumnIndex == 5 && isTownSortable ? c.IsActive.ToString() :
                                                           sortColumnIndex == 6 && isTownSortable ? c.Ref.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredUser = filteredUser.OrderBy(orderingFunction);
            else
                filteredUser = filteredUser.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredUser.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.UserId), c.FullName, c.Email, c.IP, c.Amount.ToString(), c.Ref.ToString(), c.IsOnline.ToString(), c.IsActive.ToString() };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allUser.Count(),
                iTotalDisplayRecords = filteredUser.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
    }
}