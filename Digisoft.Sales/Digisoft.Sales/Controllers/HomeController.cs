using Digisoft.Sales.Attributes;
using Digisoft.Sales.Models;
using Digisoft.Sales.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Controllers
{
    [ExceptionHandler]
    public class HomeController : Controller
    {
        private SalesEntities _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly BiddingService _biddingService;
        private readonly BillingService _billingService;
        private readonly JobService _jobService;

        public HomeController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            _context = new SalesEntities();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _biddingService = new BiddingService();
            _billingService = new BillingService();
            _jobService = new JobService();
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (User.IsInRole("Admin"))
            {
                return View();
            }
            else
            {
                return View("User");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region User Dashboard  
        public JsonResult GetUserDashboardData(DateTime? Date)
        {
            var date = (DateTime)(Date.HasValue ? Date : DateTime.Now);

            var vm = new DashboardVM();
            var user = UserManager.FindById(User.Identity.GetUserId());
            var totalBidder = 0;
            //bids(job) applied by current bidder 
            var jobs = _jobService.GetTotalJobsOfMonth((DateTime)Date).ToList();
            var biddings = _biddingService.GetTotalBiddingsOfMonth((DateTime)Date).ToList();
            if (biddings != null)
            {
                vm.TotalUserBiddings = biddings.Where(x => x.UserId == user.Id).Count();
                vm.TotalBiiddings = biddings.Count();
                totalBidder = _context.AspNetUsers.Count(x => x.AspNetRoles.Any(y => y.Name == "Bidder"));
                vm.JobAppliedAverage = Utility.StaticMethods.GetAverage(totalBidder, vm.TotalBiiddings);
                var diff1 = vm.TotalUserBiddings - vm.JobAppliedAverage;
                vm.AppliedJobsResult = vm.TotalUserBiddings < vm.JobAppliedAverage
                    ? string.Format("Less then {0} from average", Convert.ToString(Math.Abs(diff1)))
                    : diff1 == 0
                    ? string.Format("{0} from average", Convert.ToString(Math.Abs(diff1)))
                    : string.Format("More then {0} from average", Convert.ToString(Math.Abs(diff1)));


                //Connects Used
                vm.TotalConnectsUsedByUser = biddings.Where(x => x.UserId == user.Id).Sum(i => i.ConnectsUsed);
                vm.TotalUsedConnects = biddings.Sum(i => i.ConnectsUsed);
                vm.UsedConnectsAverage = Utility.StaticMethods.GetAverage(totalBidder, vm.TotalUsedConnects);
                var diff2 = vm.TotalConnectsUsedByUser - vm.UsedConnectsAverage;
                vm.UsedConnectsResult = vm.TotalConnectsUsedByUser < vm.UsedConnectsAverage
                    ? string.Format("Less then {0} from average", Convert.ToString(Math.Abs(diff2)))
                    : diff2 == 0
                        ? string.Format("{0} from average", Convert.ToString(Math.Abs(diff2)))
                        : string.Format("More then {0} from average", Convert.ToString(Math.Abs(diff2)));


                //Replies
                vm.TotalRepliesOfUser = biddings.Where(x => x.UserId == user.Id && x.GetReply == true).Count();
                vm.TotalReplies = biddings.Where(x => x.GetReply == true).Count();
                vm.RepliesAverage = Utility.StaticMethods.GetAverage(totalBidder, vm.TotalReplies);
                var diff3 = vm.TotalRepliesOfUser - vm.RepliesAverage;
                vm.RepliesResult = vm.TotalRepliesOfUser < vm.RepliesAverage
                    ? string.Format("Less then {0} from average", Convert.ToString(Math.Abs(diff3)))
                    : diff3 == 0
                        ? string.Format("{0} from average", Convert.ToString(Math.Abs(diff3)))
                        : string.Format("More then {0} from average", Convert.ToString(Math.Abs(diff3)));
            }

            if (jobs != null)
            {
                vm.TotalJobs = jobs.Count();
                vm.TotalJobsOfUser = jobs.Where(x => x.UserId == user.Id).Count();
                vm.TotalJobsAverage = Utility.StaticMethods.GetAverage(totalBidder, vm.TotalJobs);

                var diff4 = vm.TotalJobsOfUser - vm.TotalJobsAverage;
                vm.JobsResult = vm.TotalJobsOfUser < vm.TotalJobsAverage
                    ? string.Format("Less then {0} from average", Convert.ToString(Math.Abs(diff4)))
                    : diff4 == 0
                        ? string.Format("{0} from average", Convert.ToString(Math.Abs(diff4)))
                        : string.Format("More then {0} from average", Convert.ToString(Math.Abs(diff4)));
            }
            return Json(new
            {
                Success = true,
                data = vm,
            }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region Admin dashboard
        public JsonResult GetProgressData(DateTime? Date)
        {
            DateTime date = (DateTime)(Date.HasValue ? Date : DateTime.Now);
            DateTime firstDateOfYear;
            var isEmpty = false;

            var vm = new UserProgressVm();
            vm.Datasets = new List<Datasets>();
            var connectsDataSet = new Datasets();
            var jobDataSet = new Datasets();
            var repliesDataSet = new Datasets();

            var user = UserManager.FindById(User.Identity.GetUserId());
            vm.Labels = Utility.StaticMethods.GetListOfMonths();
            var totalMonths = vm.Labels?.Count();

            connectsDataSet.label = "Used Connets";
            connectsDataSet.backgroundColor = "rgb(255, 99, 132)";
            connectsDataSet.borderColor = "rgb(255, 99, 132)";
            connectsDataSet.fill = false;
            connectsDataSet.data = new List<int>();

            jobDataSet.label = "Jobs";
            jobDataSet.backgroundColor = "rgb(255, 205, 86)";
            jobDataSet.borderColor = "rgb(255, 205, 86)";
            jobDataSet.fill = false;
            jobDataSet.data = new List<int>();

            repliesDataSet.label = "Replies";
            repliesDataSet.backgroundColor = "rgb(54, 162, 235)";
            repliesDataSet.borderColor = "rgb(54, 162, 235)";
            repliesDataSet.fill = false;
            repliesDataSet.data = new List<int>();

            //bids(job) applied by current bidder 
            var jobs = _jobService.GetUserTotalJobsOfYear(date, user.Id).ToList();
            var biddings = _biddingService.GetUserTotalBiddingsOfYear(date, user.Id).ToList();

            if (biddings != null || jobs != null)
            {
                //fill up the datasets 
                firstDateOfYear = new DateTime(date.Year, 1, 1);
                for (var month = 1; month <= totalMonths; month++)
                {

                    if (biddings != null)
                    {
                        //connects dataset
                        var connectsUsed = biddings.Where(x => x.CreatedOn.Month == firstDateOfYear.Month && x.CreatedOn.Year == firstDateOfYear.Year)
                            .Sum(x => x.ConnectsUsed);
                        connectsDataSet.data.Add(connectsUsed);

                        //replies dataset
                        var replies = biddings.Where(x => x.CreatedOn.Month == firstDateOfYear.Month && x.CreatedOn.Year == firstDateOfYear.Year && x.GetReply == true)
                                      .Count();
                        repliesDataSet.data.Add(replies);
                    }
                    else
                    {
                        connectsDataSet.data.Add(default);
                        repliesDataSet.data.Add(default);
                    }

                    if (jobs != null)
                    {
                        //replies dataset
                        var jobCount = jobs.Where(x => x.CreatedOn.Month == firstDateOfYear.Month && x.CreatedOn.Year == firstDateOfYear.Year)
                                    .Count();
                        jobDataSet.data.Add(jobCount);
                    }
                    else
                    {
                        jobDataSet.data.Add(default);
                    }

                    //prevent date to exceed last date
                    if (month != totalMonths)
                        firstDateOfYear = firstDateOfYear.AddMonths(1);
                }
            }

            //if(connectsDataSet.data.Sum() == 0 && jobDataSet.data.Sum() == 0 && repliesDataSet.data.Sum() == 0)
            //{
            //    isEmpty = true;
            //} 

            vm.Datasets.Add(connectsDataSet);
            vm.Datasets.Add(repliesDataSet);
            vm.Datasets.Add(jobDataSet);
            return Json(new
            {
                Success = true,
                data = vm,
            }, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// GetAllBidderData for admin
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public JsonResult GetAllBidderPerformanceData(DateTime? Date)
        {
            DateTime date = (DateTime)(Date.HasValue ? Date : DateTime.Now);

            var vm = new UserProgressVm();
            vm.Datasets = new List<Datasets>();
            var bidsDataSet = new Datasets();
            var jobDataSet = new Datasets();
            var repliesDataSet = new Datasets();
            vm.Labels = new List<string>();

            bidsDataSet.label = "Bids";
            bidsDataSet.backgroundColor = "rgba(255, 99, 132, 0.5)";
            bidsDataSet.borderColor = "rgb(255, 99, 132)";
            //bidsDataSet.fill = false;
            bidsDataSet.data = new List<int>();

            jobDataSet.label = "Hired";
            jobDataSet.backgroundColor = "rgba(54, 162, 235, 0.5)";
            jobDataSet.borderColor = "rgb(54, 162, 235)";
            //jobDataSet.fill = false;
            jobDataSet.data = new List<int>();

            repliesDataSet.label = "Replies";
            repliesDataSet.backgroundColor = "rgba(153, 102, 255, 0.5)";
            repliesDataSet.borderColor = "rgb(153, 102, 255)";
            //repliesDataSet.fill = false;
            repliesDataSet.data = new List<int>();

            //bids(job) applied by current bidder 
            var jobs = _jobService.GetBiddersTotalJobsOfMonth(date).ToList();

            var biddings = _biddingService.GetBiddersTotalBiddingsOfMonth(date).ToList();
            //var bidders = _context.AspNetUsers.Where(x => x.AspNetRoles.Any(y => y.Name == "Bidder")).ToList();
            //vm.Labels = bidders?.Select(x => x.UserName).ToList();
            var bidders = _context.UserDetails.Where(x => x.AspNetUser.AspNetRoles.Any(y => y.Name == "Bidder")).ToList();
            var totalBidder = bidders?.Count();
            vm.Labels = bidders?.Select(x => string.Format("{0} {1}", x.FirstName, x.LastName)).ToList();

            if (biddings != null || jobs != null)
            {
                if (biddings != null)
                {
                    foreach (var bidder in bidders)
                    {
                        if (biddings != null)
                        {
                            //bids dataset
                            var bids = biddings.Where(x => x.UserId == bidder.UserId)
                                .Count();
                            bidsDataSet.data.Add(bids);

                            //replies dataset
                            var replies = biddings.Where(x => x.UserId == bidder.UserId && x.GetReply == true)
                                          .Count();
                            repliesDataSet.data.Add(replies);
                        }
                        else
                        {
                            bidsDataSet.data.Add(default);
                            repliesDataSet.data.Add(default);
                        }

                        if (jobs != null)
                        {
                            //replies dataset
                            var jobCount = jobs.Where(x => x.UserId == bidder.UserId)
                                        .Count();
                            jobDataSet.data.Add(jobCount);
                        }
                        else
                        {
                            jobDataSet.data.Add(default);
                        }
                    }
                }
            }

            vm.Datasets.Add(bidsDataSet);
            vm.Datasets.Add(repliesDataSet);
            vm.Datasets.Add(jobDataSet);
            return Json(new
            {
                Success = true,
                data = vm,
            }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpGet]
        public JsonResult GetList(DataTablesParam param, string sortDir, string sortCol, DateTime? startDate, DateTime? endDate)
        {
            var vm = new List<BidderVM>();
            int pageNo = 1;
            int totalCount = 0;
            var bidders = _context.UserDetails.Where(x => x.AspNetUser.AspNetRoles.Any(y => y.Name == "Bidder")).ToList();

            if (param.iDisplayStart >= param.iDisplayLength)
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            var data = _biddingService.GetBiddersData(startDate, endDate, bidders);
            totalCount = data.Count();
            vm = data
                    .OrderBy(x => x.BidderName)
                    .Skip((pageNo - 1) * param.iDisplayLength)
                    .Take(param.iDisplayLength).ToList();

            return Json(new
            {
                aaData = vm,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalCount,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Bidders total billings of the month
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBiddersBillings(DateTime? Date)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var vms = new List<BidderVM>();
            var date = (DateTime)(Date.HasValue ? Date : DateTime.Now);
            int totalCount = 0;
            var bidderVm = new BidderBillingVm();
            var labels = new List<string>();
            var totalPercentage = new List<decimal>();
            var bidders = _context.UserDetails.Where(x => x.AspNetUser.AspNetRoles.Any(y => y.Name == "Bidder")).ToList();
            var billings = _billingService.GetBiddersTotalBillings(date).ToList();
            decimal TotalBillingAmount = 0;

            if (billings.Any())
            {
                var fixedTotalBillingAmount = billings.Where(x => x.Amount.HasValue).Sum(s => s.JobPrice);
                var hourlyTotalBillingAmount = billings.Where(x => x.Hours.HasValue).Select(s => new { TotalAmount = s.Hours * s.JobPrice }).Sum(a => a.TotalAmount);
                TotalBillingAmount = fixedTotalBillingAmount +
                    (hourlyTotalBillingAmount.HasValue ? (decimal)hourlyTotalBillingAmount : 0);
                if (TotalBillingAmount > 0)
                {
                    foreach (var bidder in bidders)
                    {
                        var bidderBilling = new List<BillingViewModel>();
                        bidderBilling = billings.Where(x => x.Job.UserId == bidder.UserId).ToList();
                        var fixedBillingAmount = bidderBilling.Where(x => x.Amount.HasValue).Sum(s => s.JobPrice);
                        var hourlyBillingAmount = bidderBilling.Where(x => x.Hours.HasValue).Select(s => new { TotalAmount = s.Hours * s.JobPrice }).Sum(a => a.TotalAmount);
                        var sum = fixedBillingAmount + hourlyBillingAmount;

                        labels.Add(bidder.FirstName);
                        totalPercentage.Add(Convert.ToDecimal(sum));
                    }
                }
            }
            bidderVm.Labels = labels;
            bidderVm.TotalPaidPercentage = totalPercentage;
            totalCount = totalPercentage.Count();
            return Json(new
            {
                Success = true,
                TotalCount = totalCount,
                data = bidderVm,
            }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion
    }
}