using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Models
{
    public class AddEditBiddingViewModel: BaseModel
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
        public string TeamLead { get; set; }
        public Nullable<int> TeamLeadId { get; set; }
        public Nullable<bool> GetReply { get; set; }
        [System.ComponentModel.DataAnnotations.DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}",ConvertEmptyStringToNull = true, ApplyFormatInEditMode = true)]
        public DateTime AppliedOn { get; set; }
        public String AppliedDate { get; set; }
        public string UserName { get; set; }
        public string PlatformName { get; set; }
        public string AppliedUnderName { get; set; }
        public string DeveloperName { get; set; }
        public string ProjectTypeName { get; set; }
        public string TechnologyName { get; set; }
        public List<SelectListItem> Platforms { get; set; }
        public List<SelectListItem> AppliedUnders { get; set; }
        public List<SelectListItem> Developers { get; set; }
        public List<SelectListItem> ProjectTypes { get; set; }
        public List<SelectListItem> Technologies { get; set; }
        public List<SelectListItem> TeadLeads { get; set; }
        public List<SelectListItem> Bidders { get; set; }
        public List<string> BiddersIds { get; set; }
    }
}