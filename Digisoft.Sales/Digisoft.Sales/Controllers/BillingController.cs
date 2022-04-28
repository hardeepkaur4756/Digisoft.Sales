using Digisoft.Sales.Attributes;
using Digisoft.Sales.Helper;
using Digisoft.Sales.Models;
using Digisoft.Sales.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Controllers
{
    [ExceptionHandler]
    public class BillingController : Controller
    {
        #region properties

        private SalesEntities _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly BillingService _billingService;

        #endregion

        public BillingController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            _context = new SalesEntities();
            _billingService = new BillingService();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        // GET: Billing
        public ActionResult Index()
        {
            var vm = new BillingViewModel();
            vm.Clients = _context.Clients.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return View("Index", vm);
        }

        [HttpGet]
        public JsonResult GetList(DataTablesParam param, string sortDir, string sortCol, string[] ids, DateTime? startDate, DateTime? endDate)
        {
            var billingsVm = new List<BillingViewModel>();
            int pageNo = 1;
            var clientIds = !string.IsNullOrEmpty(ids[0].ToString()) ? ids[0].ToString().Split(',').ToList() : new List<string>();
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (param.iDisplayStart >= param.iDisplayLength)
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;

            int totalCount = 0;

            if (param.sSearch != null)
            {
                //sortCol = sortCol == "JobTitle" ? "Job.Title" : sortCol;
                sortCol = sortCol == "CreatedByName" ? "AspNetUser.UserName" : sortCol;
                var billings = _billingService.GetAllAfterSearch(param, startDate, endDate, clientIds)
                     .OrderBy(x => x.Id)
                     .OrderBy(sortCol + " " + sortDir)
                     .Skip((pageNo - 1) * param.iDisplayLength)
                     .Take(param.iDisplayLength)
                     .ToList();
                AutoMapper.Mapper.Map(billings, billingsVm);
                foreach (var item in billingsVm)
                {
                    item.IsCurrentUser = user.Id == item.UserId ? true : false;
                }
                totalCount = billingsVm.Count();

            }
            else
            {
                sortCol = sortCol == "JobTitle" ? "Job.Title" : sortCol;
                sortCol = sortCol == "CreatedByName" ? "AspNetUser.UserName" : sortCol;
                totalCount = _billingService.GetAll(startDate, endDate, clientIds).Count();
                var billings = _billingService.GetAll(startDate, endDate, clientIds)
                    .OrderBy(x => x.Id)
                    .OrderBy(sortCol + " " + sortDir)
                    .Skip((pageNo - 1) * param.iDisplayLength)
                    .Take(param.iDisplayLength)
                    .ToList();
                AutoMapper.Mapper.Map(billings, billingsVm);
                foreach (var item in billingsVm)
                {
                    item.IsCurrentUser = user.Id == item.UserId ? true : false;
                }
            }
            return Json(new
            {
                aaData = billingsVm,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalCount,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddBilling(int id, string viewType)
        {
            BillingViewModel vm = id > 0 ? _billingService.GetByIDVM(id) : new BillingViewModel();
            if (viewType == "Display")
            {
                vm.ViewType = "Display";
            }
            else
            {
                vm.BillingDate = id > 0 ? vm.BillingDate : DateTime.Now;
                var jobs = _context.Jobs.ToList();
                vm.Jobs = jobs.Select(x => new SelectListItem { Text = string.Format("{0}-{1}", x.Client.Name, x.Developer.Name), Value = x.Id.ToString() })
                    .OrderBy(x => x.Text)
                    .ToList();
            }
            return Json(new { Success = true, Html = this.RenderPartialViewToString("_AddEditBilling", vm) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Insert update billing
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertUpdate(BillingViewModel vm)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (vm.Id < 1)
            {
                vm.UserId = user.Id;
            }
            _billingService.InsertUpdate(vm);
            if (vm.Id > 0)
            {
                return Json(new { Message = "Billing updated successfully!", Success = true });
            }
            else
            {
                return Json(new { Message = "Billing inserted successfully!", Success = true });
            }
        }
        /// <summary>
        /// delete billing
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            _billingService.Delete(Id);
            return Json(new { Message = "Billing deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProjectType(int id)
        {
            var projectTypeId = Convert.ToString(_context.Jobs.FirstOrDefault(x => x.Id == id).ProjectTypeId);
            if (!string.IsNullOrEmpty(projectTypeId))
            {
                return Json(new { Success = true, ProjectTypeId = projectTypeId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, ProjectTypeId = projectTypeId }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}