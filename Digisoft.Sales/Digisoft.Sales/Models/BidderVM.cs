using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class BidderVM
    {
        public String BidderName { get; set; }
        public int Jobs { get; set; }
        public int Hires { get; set; }
        public int ConnectsUsed { get; set; }
        public int Replies { get; set; }
    }


}