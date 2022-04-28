using Digisoft.Sales.Attributes;
using Digisoft.Sales.Helper;
using Digisoft.Sales.Models;
using Digisoft.Sales.Service;
using Digisoft.Sales.Service.Interface;
using Digisoft.Sales.Utility.Enums;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Controllers
{
    [Authorize]
    [ExceptionHandler]
    public class BiddingController : Controller
    {
        #region properties

        private SalesEntities _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly BiddingService _biddingService;
        private readonly BillingService _billingService;
        private readonly JobService _jobService;
        #endregion

        public BiddingController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            _context = new SalesEntities();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _biddingService = new BiddingService();
            _billingService = new BillingService();
            _jobService = new JobService();
        }

        // GET: Bidding
        public ActionResult Index()
        {
            var vm = new AddEditBiddingViewModel();
            vm.Bidders = _context.UserDetails.Where(x => x.AspNetUser.AspNetRoles.Any(y => y.Name == "Bidder")).Select(x => new SelectListItem { Text = x.FirstName, Value = x.UserId.ToString() }).OrderBy(x => x.Text).ToList();
            return View("Index", vm);
        }

        [HttpGet]
        public JsonResult GetList(DataTablesParam param, string sortDir, string sortCol, string[] ids, DateTime? startDate, DateTime? endDate)
       {
            var biddingsVm = new List<BiddingViewModel>();
            var biddings = new List<Bidding>();
            int pageNo = 1;
            var biddersIds = !string.IsNullOrEmpty(ids[0].ToString())? ids[0].ToString().Split(',').ToList():new List<string>();
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

                    case "PlatformName":
                        sortCol = sortCol == "PlatformName" ? "Platform.Name" : sortCol;
                        break;
                    case "TeamLeadName":
                        sortCol = sortCol == "TeamLeadName" ? "TeamLead.Name" : sortCol;
                        break;
                    case "TechnologyName":
                        sortCol = sortCol == "TechnologyName" ? "Technology.Name" : sortCol;

                        break;
                    case "AppliedUnderName":
                        sortCol = sortCol == "AppliedUnderName" ? "AppliedUnder.Name" : sortCol;
                        break;
                    default:
                        break;
                }
                if (startDate.HasValue && endDate.HasValue)
                {
                    totalCount = _biddingService.GetAllBetweenDatesAfterSearch(param, (DateTime)startDate, (DateTime)endDate, biddersIds).Count();
                    if (sortCol == "TeamLead.Name")
                    {
                        if (sortDir == "asc")
                        {
                            biddings = _biddingService.GetAllBetweenDatesAfterSearch(param, (DateTime)startDate, (DateTime)endDate, biddersIds)
                                .OrderBy(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength)
                                .Take(param.iDisplayLength).ToList();
                        }
                        if (sortDir == "desc")
                        {
                            biddings = _biddingService.GetAllBetweenDatesAfterSearch(param, (DateTime)startDate, (DateTime)endDate,biddersIds)
                                .OrderByDescending(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength)
                                .Take(param.iDisplayLength).ToList();
                        }

                        biddingsVm = biddings
                       .Select(x => new BiddingViewModel
                       {
                           AppliedOn = x.AppliedOn,
                           AppliedUnderId = x.AppliedUnderId,
                           Id = x.Id,
                           AppliedUnderName = x.AppliedUnder.Name,
                           ConnectsUsed = x.ConnectsUsed,
                           CreatedOn = x.CreatedOn,
                           DeveloperId = x.DeveloperId,
                           DeveloperName = x.Developer?.Name,
                           JobUrl = x.JobUrl,
                           Notes = x.Notes,
                           PlatformId = x.PlatformId,
                           PlatformName = x.Platform?.Name,
                           Price = x.Price,
                           ProjectTypeId = x.ProjectTypeId,
                           ProjectTypeName = x.ProjectType?.Name,
                           ProposalUrl = x.ProposalUrl,
                           TechnologyId = x.TechnologyId,
                           TechnologyName = x.Technology?.Name,
                           Title = x.Title,
                           UserId = x.UserId,
                       //UserName = x.AspNetUser?.UserName,
                       UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                           IsCurrentUser = user.Id == x.UserId ? true : false,
                           IsUnderDeleteTime = ((DateTime.Now - x.CreatedOn).TotalDays < 7) ? true : false,
                           TeamLead = x.TeamLead?.Name,
                           GetReply = x.GetReply ?? false
                       })
                       .ToList();
                    }
                    else
                    {
                        biddingsVm = _biddingService.GetAllBetweenDatesAfterSearch(param, (DateTime)startDate, (DateTime)endDate,biddersIds)
                        .OrderBy(x => x.Id)
                        .OrderBy(sortCol + " " + sortDir)
                        .Skip((pageNo - 1) * param.iDisplayLength)
                        .Take(param.iDisplayLength)
                        .Select(x => new BiddingViewModel
                        {
                            AppliedOn = x.AppliedOn,
                            AppliedUnderId = x.AppliedUnderId,
                            Id = x.Id,
                            AppliedUnderName = x.AppliedUnder.Name,
                            ConnectsUsed = x.ConnectsUsed,
                            CreatedOn = x.CreatedOn,
                            DeveloperId = x.DeveloperId,
                            DeveloperName = x.Developer?.Name,
                            JobUrl = x.JobUrl,
                            Notes = x.Notes,
                            PlatformId = x.PlatformId,
                            PlatformName = x.Platform?.Name,
                            Price = x.Price,
                            ProjectTypeId = x.ProjectTypeId,
                            ProjectTypeName = x.ProjectType?.Name,
                            ProposalUrl = x.ProposalUrl,
                            TechnologyId = x.TechnologyId,
                            TechnologyName = x.Technology?.Name,
                            Title = x.Title,
                            UserId = x.UserId,
                        //UserName = x.AspNetUser?.UserName,
                        UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                            IsCurrentUser = user.Id == x.UserId ? true : false,
                            TeamLead = x.TeamLead?.Name,
                            GetReply = x.GetReply ?? false
                        })
                        .ToList();
                    }
                }
                else
                {
                    totalCount = _biddingService.GetAllAfterSearch(param, biddersIds).Count();
                    if (sortCol == "TeamLead.Name")
                    {
                        if (sortDir == "asc")
                        {
                            biddings = _biddingService.GetAllAfterSearch(param, biddersIds)
                                .OrderBy(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength)
                                .Take(param.iDisplayLength).ToList();
                        }
                        if (sortDir == "desc")
                        {
                            biddings = _biddingService.GetAllAfterSearch(param, biddersIds)
                                .OrderByDescending(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength)
                                .Take(param.iDisplayLength).ToList();
                        }

                        biddingsVm = biddings
                       .Select(x => new BiddingViewModel
                       {
                           AppliedOn = x.AppliedOn,
                           AppliedUnderId = x.AppliedUnderId,
                           Id = x.Id,
                           AppliedUnderName = x.AppliedUnder.Name,
                           ConnectsUsed = x.ConnectsUsed,
                           CreatedOn = x.CreatedOn,
                           DeveloperId = x.DeveloperId,
                           DeveloperName = x.Developer?.Name,
                           JobUrl = x.JobUrl,
                           Notes = x.Notes,
                           PlatformId = x.PlatformId,
                           PlatformName = x.Platform?.Name,
                           Price = x.Price,
                           ProjectTypeId = x.ProjectTypeId,
                           ProjectTypeName = x.ProjectType?.Name,
                           ProposalUrl = x.ProposalUrl,
                           TechnologyId = x.TechnologyId,
                           TechnologyName = x.Technology?.Name,
                           Title = x.Title,
                           UserId = x.UserId,
                       //UserName = x.AspNetUser?.UserName,
                       UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                           IsCurrentUser = user.Id == x.UserId ? true : false,
                           IsUnderDeleteTime = ((DateTime.Now - x.CreatedOn).TotalDays < 7) ? true : false,
                           TeamLead = x.TeamLead?.Name,
                           GetReply = x.GetReply ?? false
                       })
                       .ToList();
                        totalCount = biddingsVm.Count();
                    }
                    else
                    {
                        biddingsVm = _biddingService.GetAllAfterSearch(param, biddersIds)
                        .OrderBy(x => x.Id)
                        .OrderBy(sortCol + " " + sortDir)
                        .Skip((pageNo - 1) * param.iDisplayLength)
                        .Take(param.iDisplayLength)
                        .Select(x => new BiddingViewModel
                        {
                            AppliedOn = x.AppliedOn,
                            AppliedUnderId = x.AppliedUnderId,
                            Id = x.Id,
                            AppliedUnderName = x.AppliedUnder.Name,
                            ConnectsUsed = x.ConnectsUsed,
                            CreatedOn = x.CreatedOn,
                            DeveloperId = x.DeveloperId,
                            DeveloperName = x.Developer?.Name,
                            JobUrl = x.JobUrl,
                            Notes = x.Notes,
                            PlatformId = x.PlatformId,
                            PlatformName = x.Platform?.Name,
                            Price = x.Price,
                            ProjectTypeId = x.ProjectTypeId,
                            ProjectTypeName = x.ProjectType?.Name,
                            ProposalUrl = x.ProposalUrl,
                            TechnologyId = x.TechnologyId,
                            TechnologyName = x.Technology?.Name,
                            Title = x.Title,
                            UserId = x.UserId,
                        //UserName = x.AspNetUser?.UserName,
                        UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                            IsCurrentUser = user.Id == x.UserId ? true : false,
                            TeamLead = x.TeamLead?.Name,
                            GetReply = x.GetReply ?? false
                        })
                        .ToList();
                    }
                }
            }
            else
            {
                switch (sortCol)
                {
                    case "UserName":
                        sortCol = sortCol == "UserName" ? "AspNetUser.UserName" : sortCol;
                        break;

                    case "PlatformName":
                        sortCol = sortCol == "PlatformName" ? "Platform.Name" : sortCol;
                        break;
                    case "TeamLead":
                        sortCol = sortCol == "TeamLead" ? "TeamLead.Name" : sortCol;
                        break;
                    case "TechnologyName":
                        sortCol = sortCol == "TechnologyName" ? "Technology.Name" : sortCol;

                        break;
                    case "AppliedUnderName":
                        sortCol = sortCol == "AppliedUnderName" ? "AppliedUnder.Name" : sortCol;
                        break;
                    default:
                        break;
                }

                totalCount = _biddingService.GetAll().Count();

                if (sortCol == "TeamLead.Name")
                {
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        totalCount = _biddingService.GetAllBetweenDates((DateTime)startDate, (DateTime)endDate, biddersIds).Count();
                        if (sortDir == "asc")
                        {
                            biddings = _biddingService.GetAllBetweenDates((DateTime)startDate, (DateTime)endDate,biddersIds).OrderBy(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                        }
                        if (sortDir == "desc")
                        {
                            biddings = _biddingService.GetAllBetweenDates((DateTime)startDate, (DateTime)endDate, biddersIds).OrderByDescending(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                        }
                    }
                    else
                    {
                        if (sortDir == "asc")
                        {
                            biddings = _biddingService.GetAll().OrderBy(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                        }
                        if (sortDir == "desc")
                        {
                            biddings = _biddingService.GetAll().OrderByDescending(x => x.TeamLead?.Name)
                                .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                        }
                    }
                    biddingsVm = biddings
                        .Select(x => new BiddingViewModel
                        {
                            AppliedOn = x.AppliedOn,
                            AppliedUnderId = x.AppliedUnderId,
                            Id = x.Id,
                            AppliedUnderName = x.AppliedUnder?.Name,
                            ConnectsUsed = x.ConnectsUsed,
                            CreatedOn = x.CreatedOn,
                            DeveloperId = x.DeveloperId,
                            DeveloperName = x.Developer?.Name,
                            JobUrl = x.JobUrl,
                            Notes = x.Notes,
                            PlatformId = x.PlatformId,
                            PlatformName = x.Platform?.Name,
                            Price = x.Price,
                            ProjectTypeId = x.ProjectTypeId,
                            ProjectTypeName = x.ProjectType?.Name,
                            ProposalUrl = x.ProposalUrl,
                            TechnologyId = x.TechnologyId,
                            TechnologyName = x.Technology?.Name,
                            Title = x.Title,
                            UserId = x.UserId,
                            UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                            //UserName = x.AspNetUser?.UserName,
                            IsCurrentUser = user.Id == x.UserId ? true : false,
                            IsUnderDeleteTime = ((DateTime.Now - x.CreatedOn).TotalDays < 7) ? true : false,
                            TeamLead = x.TeamLead?.Name,
                            GetReply = x.GetReply ?? false
                        }).ToList();
                }
                else
                {
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        totalCount = _biddingService.GetAllBetweenDates((DateTime)startDate, (DateTime)endDate, biddersIds).Count();
                        biddingsVm = _biddingService.GetAllBetweenDates((DateTime)startDate, (DateTime)endDate, biddersIds).OrderBy(x => x.Id).OrderBy(sortCol + " " + sortDir)
                       .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                        .Select(x => new BiddingViewModel
                        {
                            AppliedOn = x.AppliedOn,
                            AppliedUnderId = x.AppliedUnderId,
                            Id = x.Id,
                            AppliedUnderName = x.AppliedUnder?.Name,
                            ConnectsUsed = x.ConnectsUsed,
                            CreatedOn = x.CreatedOn,
                            DeveloperId = x.DeveloperId,
                            DeveloperName = x.Developer?.Name,
                            JobUrl = x.JobUrl,
                            Notes = x.Notes,
                            PlatformId = x.PlatformId,
                            PlatformName = x.Platform?.Name,
                            Price = x.Price,
                            ProjectTypeId = x.ProjectTypeId,
                            ProjectTypeName = x.ProjectType?.Name,
                            ProposalUrl = x.ProposalUrl,
                            TechnologyId = x.TechnologyId,
                            TechnologyName = x.Technology?.Name,
                            Title = x.Title,
                            UserId = x.UserId,
                            //UserName = x.AspNetUser?.UserName,
                            UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                            IsCurrentUser = user.Id == x.UserId ? true : false,
                            IsUnderDeleteTime = ((DateTime.Now - x.CreatedOn).TotalDays < 7) ? true : false,
                            TeamLead = x.TeamLead?.Name,
                            GetReply = x.GetReply ?? false
                        }).ToList();
                    }
                    else
                    {
                        totalCount = _biddingService.GetAll(biddersIds).Count();
                        biddingsVm = _biddingService.GetAll(biddersIds).OrderBy(x => x.Id).OrderBy(sortCol + " " + sortDir)
                            .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                             .Select(x => new BiddingViewModel
                             {
                                 AppliedOn = x.AppliedOn,
                                 AppliedUnderId = x.AppliedUnderId,
                                 Id = x.Id,
                                 AppliedUnderName = x.AppliedUnder?.Name,
                                 ConnectsUsed = x.ConnectsUsed,
                                 CreatedOn = x.CreatedOn,
                                 DeveloperId = x.DeveloperId,
                                 DeveloperName = x.Developer?.Name,
                                 JobUrl = x.JobUrl,
                                 Notes = x.Notes,
                                 PlatformId = x.PlatformId,
                                 PlatformName = x.Platform?.Name,
                                 Price = x.Price,
                                 ProjectTypeId = x.ProjectTypeId,
                                 ProjectTypeName = x.ProjectType?.Name,
                                 ProposalUrl = x.ProposalUrl,
                                 TechnologyId = x.TechnologyId,
                                 TechnologyName = x.Technology?.Name,
                                 Title = x.Title,
                                 UserId = x.UserId,
                                 //UserName = x.AspNetUser?.UserName,
                                 UserName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                                 IsCurrentUser = user.Id == x.UserId ? true : false,
                                 IsUnderDeleteTime = ((DateTime.Now - x.CreatedOn).TotalDays < 7) ? true : false,
                                 TeamLead = x.TeamLead?.Name,
                                 GetReply = x.GetReply ?? false
                             }).ToList();
                    }
                }
            }
            //Comment due to performance issue
            //AutoMapper.Mapper.Map(bidding, biddingsVm);

            return Json(new
            {
                aaData = biddingsVm,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalCount,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddBidding(int id, string viewType)
        {
            AddEditBiddingViewModel vm = id > 0 ? _biddingService.GetByIDVM(id) : new AddEditBiddingViewModel();

            if (viewType == "Display")
            {
                vm.ViewType = "Display";
            }
            else
            {
                vm.AppliedOn = id > 0 ? vm.AppliedOn : DateTime.Now;
                vm.AppliedUnders = _context.AppliedUnders.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Developers = _context.Developers.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Platforms = _context.Platforms.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.ProjectTypes = _context.ProjectTypes.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                vm.Technologies = _context.Technologies.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                //vm.TeadLeads = _context.TeamLeads.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            }
            return Json(new { Success = true, Html = this.RenderPartialViewToString("_AddEditBidding", vm) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddBiddingTeamLead(int id)
        {
            AddEditBiddingViewModel vm = id > 0 ? _biddingService.GetByIDVM(id) : new AddEditBiddingViewModel();
            vm.TeadLeads = _context.TeamLeads.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(new { Success = true, Html = this.RenderPartialViewToString("_AddEditBiddingTeamLead", vm) }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Insert update user
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertUpdate(AddEditBiddingViewModel vm)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (vm.Id < 1)
            {
                vm.UserId = user.Id;
            }
            _biddingService.InsertUpdate(vm);
            if (vm.Id > 0)
            {
                return Json(new { Message = "Bid updated successfully!", Success = true });
            }
            else
            {
                return Json(new { Message = "Bid inserted successfully!", Success = true });
            }
        }

        [HttpPost]
        public ActionResult InsertUpdateTeamLead(AddEditBiddingViewModel vm)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            vm.UserId = user.Id;
            _biddingService.InsertUpdateTeamLead(vm);
            if (vm.Id > 0)
            {
                return Json(new { Message = "TeamLead updated successfully!", Success = true });
            }
            else
            {
                return Json(new { Message = "TeamLead inserted successfully!", Success = true });
            }
        }

        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            //First verify is any job related to bidding
            var jobCount = _jobService.GetBidJobs(Id).Count();
            if (jobCount > 0)
            {
                return Json(new { Message = "Sorry ! Job exist related to it!", Success = false, IsInfo = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    _biddingService.Delete(Id);
                    return Json(new { Message = "Bid deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Check billing first
                    var billingCount = _billingService.GetBillingCount(ControllerTypeEnum.ControllerType.Bidding, Id);
                    if (billingCount <= 0)
                    {
                        _biddingService.Delete(Id);
                        return Json(new { Message = "Bid deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = "Sorry! You can't delete this record!", Success = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        #region Bid Contacts
        /// <summary>
        /// Get total contacts of the month
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTotalContact(DateTime? Date)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            var date = (DateTime)(Date.HasValue ? Date : DateTime.Now);
            int totalCount;
            var biddingContactsVm = new BiddingContactsVm();
            biddingContactsVm.Labels = new List<string>();
            biddingContactsVm.TotalContactList = new List<int>();

            var bidders = _context.UserDetails.Where(x => x.AspNetUser.AspNetRoles.Any(y => y.Name == "Bidder")).ToList();
            var totalBidder = bidders?.Count();
            biddingContactsVm.Labels = bidders?.Select(x => string.Format("{0} {1}", x.FirstName, x.LastName)).ToList();
            var biddings = _biddingService.GetBiddersTotalBiddingsOfMonth(date).ToList();

            if (biddings != null)
            {
                foreach (var item in bidders)
                {
                    //get count of used contact by bidder
                    biddingContactsVm.TotalContactList.Add((int)biddings?.Where(a => a.UserId == item.UserId).Sum(s => s.ConnectsUsed));
                }
            }
            totalCount = biddings.Count;

            return Json(new
            {
                Success = true,
                TotalCount = totalCount,
                data = biddingContactsVm,
            }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

    }
}