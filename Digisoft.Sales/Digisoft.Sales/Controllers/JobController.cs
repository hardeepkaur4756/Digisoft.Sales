using Digisoft.Sales.Attributes;
using Digisoft.Sales.Helper;
using Digisoft.Sales.Models;
using Digisoft.Sales.Service;
using Digisoft.Sales.Utility.Enums;
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
    public class JobController : Controller
    {
        #region properties

        private SalesEntities _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly JobService _jobService;
        private readonly BillingService _billingService;
        #endregion

        public JobController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            _context = new SalesEntities();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _jobService = new JobService();
            _billingService = new BillingService();
        }

        // GET: Job
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetList(DataTablesParam param,string sortDir,string sortCol, DateTime? startDate, DateTime? endDate)
        {
            var jobsVm = new List<JobViewModel>();
            int pageNo = 1;
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (param.iDisplayStart >= param.iDisplayLength)
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;

            int totalCount = 0;

            if (param.sSearch != null)
            {

                switch (sortCol)
                {

                    case "UserName":
                        sortCol = sortCol == "UserName" ? "AspNetUser.UserName" : sortCol;
                        break;
                    case "ClientName":
                       sortCol = sortCol == "ClientName" ? "Client.Name" : sortCol;
                      break;
                    case "PlatformName":
                        sortCol = sortCol == "PlatformName" ? "Platform.Name" : sortCol;
                        break;
                    case "TeamLeadName":
                        sortCol = sortCol == "TeamLeadName" ? "TeamLead.Name" : sortCol;
                        break;
                    case "BiddingTitle":
                        sortCol = sortCol == "BiddingTitle" ? "Bidding.Title" : sortCol;
                        break;
                    case "AppliedUnderName":
                        sortCol = sortCol == "AppliedUnderName" ? "AppliedUnder.Name" : sortCol;
                        break;
                    case "DeveloperName":
                        sortCol = sortCol == "DeveloperName" ? "Developer.Name" : sortCol;
                        break;
                    case "ProjectTypeName":
                        sortCol = sortCol == "ProjectTypeName" ? "ProjectType.Name" : sortCol;
                        break;
                    default:
                        break;
                }

                jobsVm = _jobService.GetAllAfterSearch(param, startDate, endDate)
                .OrderBy(x => x.Id)
                .OrderBy(sortCol + " " + sortDir)
                .Skip((pageNo - 1) * param.iDisplayLength)
                .Take(param.iDisplayLength)
                .Select(x => new JobViewModel
                {
                    AppliedUnderId = x.AppliedUnderId,
                    Id = x.Id,
                    AppliedUnderName = x.AppliedUnder.Name,
                    ClientName = x.Client.Name,
                    CreatedOn = x.CreatedOn,
                    HiredOn = x.HiredOn,
                    Price = x.Price,
                    DeveloperName = x.Developer.Name,
                    BiddingTitle = x.Bidding.Title,
                    Notes = x.Notes,
                    PlatformId = x.PlatformId,
                    PlatformName = x.Platform.Name,
                    ProjectTypeId = x.ProjectTypeId,
                    ProjectTypeName = x.ProjectType.Name,
                    TeamLeadName = x.TeamLead.Name,
                    UserId = x.UserId,
                    UserName = x.AspNetUser.UserName,
                    IsCurrentUser = user.Id == x.UserId ? true : false
                })
                .ToList();
                totalCount = jobsVm.Count();
            }
            else
            {
                switch (sortCol)
                {

                    case "UserName":
                        sortCol = sortCol == "UserName" ? "AspNetUser.UserName" : sortCol;
                        break;
                    case "ClientName":
                        sortCol = sortCol == "ClientName" ? "Client.Name" : sortCol;
                        break;
                    case "PlatformName":
                        sortCol = sortCol == "PlatformName" ? "Platform.Name" : sortCol;
                        break;
                    case "TeamLeadName":
                        sortCol = sortCol == "TeamLeadName" ? "TeamLead.Name" : sortCol;
                        break;
                    case "BiddingTitle":
                        sortCol = sortCol == "BiddingTitle" ? "Bidding.Title" : sortCol;
                        break;
                    case "AppliedUnderName":
                        sortCol = sortCol == "AppliedUnderName" ? "AppliedUnder.Name" : sortCol;
                        break;
                    case "DeveloperName":
                        sortCol = sortCol == "DeveloperName" ? "Developer.Name" : sortCol;
                        break;
                    case "ProjectTypeName":
                        sortCol = sortCol == "ProjectTypeName" ? "ProjectType.Name" : sortCol;
                        break;
                    default:
                        break;
                }
                totalCount = _jobService.GetAll(startDate, endDate).Count();
                jobsVm = _jobService.GetAll(startDate, endDate).OrderBy(x=>x.Id).OrderBy(sortCol + " " + sortDir)
                    .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                     .Select(x => new JobViewModel
                     {
                         AppliedUnderId = x.AppliedUnderId,
                         Id = x.Id,
                         AppliedUnderName = x.AppliedUnder.Name,
                         ClientName = x.Client.Name,
                         CreatedOn = x.CreatedOn,
                         HiredOn=x.HiredOn,
                         Price = x.Price,
                         DeveloperName = x.Developer.Name,
                         BiddingTitle = x.Bidding.Title,
                         Notes = x.Notes,
                         PlatformId = x.PlatformId,
                         PlatformName = x.Platform.Name,
                         ProjectTypeId = x.ProjectTypeId,
                         ProjectTypeName = x.ProjectType.Name,
                         TeamLeadName = x.TeamLead.Name,
                         UserId = x.UserId,
                         UserName = x.AspNetUser.UserName,
                         IsCurrentUser = user.Id == x.UserId ? true : false
                     }).ToList();
            }

            //Comment due to performance issue
            //AutoMapper.Mapper.Map(jobs, jobsVm);

            return Json(new
            {
                aaData = jobsVm,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalCount,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddJob
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddJob(int id, string viewType)
        {

            JobViewModel vm = id > 0 ? _jobService.GetByIDVM(id) : new JobViewModel();

            if (viewType == "Display")
            {
                vm.ViewType = "Display";
            }
            else
            {
                var userId = User.Identity.GetUserId();
                bool isLoggedInUserAdmin = UserManager.IsInRole(userId, Roles.Admin.ToString());
                var biddingNames = isLoggedInUserAdmin ? _context.Biddings : _context.Biddings.Where(x => x.UserId == userId);
                vm.HiredOn = id > 0 ? vm.HiredOn : DateTime.Now;
                vm.Biddings = biddingNames.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Users = _context.AspNetUsers.Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.AppliedUnders = _context.AppliedUnders.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Developers = _context.Developers.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Platforms = _context.Platforms.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.TeamLeaders = _context.TeamLeads.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.ProjectTypes = _context.ProjectTypes.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Clients = _context.Clients.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            }
            return Json(new { Success = true, Html = this.RenderPartialViewToString("_AddEditJob", vm) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert update user
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertUpdate(JobViewModel vm)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (vm.Id < 1)
            {
                vm.UserId = user.Id;
            }
            _jobService.InsertUpdate(vm);
            if (vm.Id > 0)
            {
                return Json(new { Message = "Job updated successfully!", Success = true });
            }
            else
            {
                return Json(new { Message = "Job inserted successfully!", Success = true });
            }

        }
        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            if (User.IsInRole("Admin"))
            {
                _jobService.Delete(Id);
                return Json(new { Message = "Job deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Check billing first
                var billingCount = _billingService.GetBillingCount(ControllerTypeEnum.ControllerType.Job, Id);
                if (billingCount <= 0)
                {
                    _jobService.Delete(Id);
                    return Json(new { Message = "Job deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Sorry! You can't delete this record!", Success = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}