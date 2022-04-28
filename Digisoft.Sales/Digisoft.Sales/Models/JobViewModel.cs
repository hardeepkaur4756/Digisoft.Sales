using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Models
{
    public class JobViewModel:BaseModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int BiddingId { get; set; }
        [Display(Name = "User")]
        public string UserId { get; set; }
        public int AppliedUnderId { get; set; }
        public int DeveloperId { get; set; }
        public int PlatformId { get; set; }
        public int TeamLeadId { get; set; }
        public int ProjectTypeId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        [Display(Name = "Hired On")]
        [System.ComponentModel.DataAnnotations.DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ConvertEmptyStringToNull = true, ApplyFormatInEditMode = true)]
        public System.DateTime HiredOn { get; set; }
        public string HiredDate { get; set; }
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        [Display(Name = "Bidding")]
        public string BiddingTitle { get; set; }
        [Display(Name = "User")]
        public string UserName { get; set; }
        [Display(Name = "Client")]
        public string ClientName { get; set; }
        [Display(Name = "Platform")]
        public string PlatformName { get; set; }
        [Display(Name = "Applied Under")]
        public string AppliedUnderName { get; set; }
        [Display(Name = "Developer")]   
        public string DeveloperName { get; set; }
        [Display(Name = "Project Type")]
        public string ProjectTypeName { get; set; }
        [Display(Name = "Team Lead")]
        public string TeamLeadName { get; set; }

        public List<SelectListItem> Biddings { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Clients { get; set; }
        public List<SelectListItem> Platforms { get; set; }
        public List<SelectListItem> AppliedUnders { get; set; }
        public List<SelectListItem> Developers { get; set; }
        public List<SelectListItem> ProjectTypes { get; set; }
        public List<SelectListItem> TeamLeaders { get; set; }
    }
}