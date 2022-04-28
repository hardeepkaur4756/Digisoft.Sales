using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class BidderBillingVm : ChartBase
    {
        public String BidderName { get; set; }
        public decimal? TotalBillingAmount { get; set; }
        public int PaidBillingPercentage { get; set; }
        public List<decimal> TotalPaidPercentage { get; set; }
    }
}