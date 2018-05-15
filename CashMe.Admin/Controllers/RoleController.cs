using CashMe.Admin.Models;
using CashMe.Service.Role;
using CashMe.Shared.Common;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        #region Ctor
        private readonly IRoleServices _roleServices;

        public RoleController(IRoleServices roleServices)
        {
            _roleServices = roleServices;
        }
        #endregion

        [Authorize(Roles =DefaultData.RoleAdmin)]
        // GET: Role       
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (!isAdminUser())
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //var Roles = context.Roles.ToList();
            //return View(Roles);
            return View();
        }
        public Boolean isAdminUser()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity;
            //    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //    var s = UserManager.GetRoles(user.GetUserId());
            //    if (s[0].ToString() == "Admin")
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            return false;
        }
       
        #region Interactive between view and controller
        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult getRoleById(string id)
        {
            return Json(_roleServices.GetRoleById(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult GetAllRoles()
        {
            return Json(_roleServices.GetAllRoles(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult Modify(string RoleName, string Id)
        {
            var result = false;
            try
            {
                var model = new IdentityRole();
                if (!Id.Equals("0"))
                    model.Id = Id;
                model.Name = RoleName;
                result = _roleServices.AddOrUpdateRole(model);
                return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Load data to form
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var roles = _roleServices.GetAllRoles();
            IEnumerable<IdentityRole> filteredUsers;
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

                filteredUsers = _roleServices.GetAllRoles().Where(aa => aa.Name.ToLower().Contains(param.sSearch.ToLower()));
                //   .Where(c => isNameSearchable && c.CategoriesName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredUsers = roles;
            }

            var isUsernameSort = Convert.ToBoolean(Request["bSortable_1"]);
            //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<IdentityRole, string> orderingFunction = (c => c.Name);

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredUsers = filteredUsers.OrderBy(orderingFunction);
            else
                filteredUsers = filteredUsers.OrderByDescending(orderingFunction);

            var displayedUsers = filteredUsers.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedUsers select new[] { c.Id, c.Name };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = roles.Count(),
                iTotalDisplayRecords = filteredUsers.Count(),
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}