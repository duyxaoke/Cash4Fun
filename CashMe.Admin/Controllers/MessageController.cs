using CashMe.Core.Data;
using CashMe.Service;
using CashMe.Service.Models;
using CashMe.Admin.Hubs;
using CashMe.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        #region Contractor
        private IMessageService MessageService;
        private IAccountServices AccountService;

        public MessageController(IMessageService MessageService, IAccountServices AccountService)
        {
            this.MessageService = MessageService;
            this.AccountService = AccountService;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                MessageService.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        // GET: Message
        public ActionResult Index()
        {
            var data = MessageService.GetMessageView().OrderByDescending(p => p.CreateDate);
            return View(data);
        }
        public ActionResult Add()
        {
            var ListUser = AccountService.GetAllUsers();
            var ListRole = AccountService.GetAllRoles();
            ViewBag.ListUser = ListUser;
            ViewBag.ListRole = ListRole;
            return View();
        }
        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult getMessageById(long id)
        {
            return Json(MessageService.GetMessage(id), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        [AjaxValidateAntiForgeryToken]
        public JsonResult Add(MessageModel model)
        {
            try
            {
                model.CreateDate = DateTime.Now;
                model.Status = 0;
                MessageService.InsertMessage(model);
                MessageService.Save();

                #region real time socket send message
                var socket = new TrackingHub();
                socket.SendMessage(true);
                #endregion

                return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);

                //throw fail;
            }
            //catch (Exception ex)
            //{
            //    return Json(new MessageResults { Status = "Error", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }
        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public JsonResult UpdateMessage(Message model)
        {
            try
            {
                var update = MessageService.GetMessage(model.Id);
                update.Status = model.Status;
                MessageService.UpdateMessage(update);
                MessageService.Save();
                return Json(new MessageResults { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new MessageResults { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxValidateAntiForgeryToken]
        public JsonResult _AutoGetMessage()
        {
            string ToUserId = "7f599448-6db3-4f81-8936-93d1498c4714";
            var Messagenew =
                MessageService.GetMessageView().Where(p => p.ToUserId == ToUserId).OrderByDescending(p => p.CreateDate)
                    .FirstOrDefault();
            return Json(Messagenew ?? null, JsonRequestBehavior.AllowGet);
        }


        [AjaxValidateAntiForgeryToken]
        public JsonResult _GetMessageByUser()
        {
            string ToUserId = "7f599448-6db3-4f81-8936-93d1498c4714";
            var data = MessageService.GetMessageView().Where(p => p.ToUserId == ToUserId).OrderByDescending(p => p.CreateDate);
            var Messagenew = data.Skip(0).Take(5);
            var CountMessage = data.Count();
            var CountMessageUnread = data.Where(c => c.Status == 0).Count();
            return Json(new { Messagenew = Messagenew ?? null, CountMessage = CountMessage, CountMessageUnread = CountMessageUnread }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var allMessage = MessageService.GetMessageView();
            IEnumerable<MessageModel> filteredMessage;
            //Check whether the Message should be filtered by keyword
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

                filteredMessage = MessageService.GetMessageView()
                   .Where(c => isNameSearchable && (c.Title.Equals(param.sSearch.ToLower()) || c.Body.Equals(param.sSearch.ToLower())));
            }
            else
            {
                filteredMessage = allMessage;
            }

            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MessageModel, string> orderingFunction = (c => sortColumnIndex == 1 && isNameSortable ? c.Id.ToString() :
                                                           sortColumnIndex == 2 && isAddressSortable ? c.Title.ToString() :
                                                           sortColumnIndex == 3 && isTownSortable ? c.Body.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredMessage = filteredMessage.OrderBy(orderingFunction);
            else
                filteredMessage = filteredMessage.OrderByDescending(orderingFunction);

            var displayedMessage = filteredMessage.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedMessage select new[] { Convert.ToString(c.Id), Convert.ToString(c.Title), Convert.ToString(c.Body), Convert.ToString(c.FromUserId), Convert.ToString(c.ToUserId), Convert.ToString(c.Body), Convert.ToString(c.CreateDate)};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allMessage.Count(),
                iTotalDisplayRecords = filteredMessage.Count(),
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }

    }
}