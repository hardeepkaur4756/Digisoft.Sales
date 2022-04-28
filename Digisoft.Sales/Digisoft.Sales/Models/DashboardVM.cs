using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class DashboardVM
    {
        public int TotalUserBiddings  { get; set; }
        public int TotalBiiddings { get; set; }
        public int TotalConnectsUsedByUser { get; set; }
        public int TotalUsedConnects { get; set; }
        public int TotalJobsOfUser { get; set; }
        public int TotalJobs { get; set; }
        public int TotalJobsAverage { get; set; }
        public int TotalRepliesOfUser { get; set; }
        public int TotalReplies { get; set; }
        public int UsedConnectsAverage{ get; set; }
        public int RepliesAverage { get; set; }
        public int JobAverage { get; set; }
        public int JobAppliedAverage { get; set; }

        public string AppliedJobsResult { get; set; }
        public string RepliesResult { get; set; }
        public string JobsResult { get; set; }
        public string UsedConnectsResult { get; set; }
    }
}