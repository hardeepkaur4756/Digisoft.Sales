using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class BiddingViewModel:BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string JobUrl { get; set; }
        public string ProposalUrl { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int PlatformId { get; set; }
        public int AppliedUnderId { get; set; }
        public int DeveloperId { get; set; }
        public int ProjectTypeId { get; set; }
        public int TechnologyId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public int ConnectsUsed { get; set; }
        public DateTime AppliedOn { get; set; }
        public string UserName { get; set; }
        public string PlatformName { get; set; }
        public string AppliedUnderName { get; set; }
        public string DeveloperName { get; set; }
        public string ProjectTypeName { get; set; }
        public string TechnologyName { get; set; }
        public bool GetReply { get; set; }
        public Nullable<int> TeamLeadId { get; set; }
        public string TeamLead{ get; set; }
        public bool IsUnderDeleteTime { get; set; }
        public int TotalContactOfEach { get; set; }
    }
}