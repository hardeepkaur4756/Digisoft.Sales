using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Models
{
    public class BillingViewModel:BaseModel
    {
        public int Id { get; set; }
        [Display(Name = "Created On")]
        public System.DateTime CreatedOn { get; set; }
        [Display(Name = "Job")]
        public int JobId { get; set; }
        [Display(Name = "Billing Date")]
        [System.ComponentModel.DataAnnotations.DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ConvertEmptyStringToNull = true, ApplyFormatInEditMode = true)]
        public System.DateTime BillingDate { get; set; }
        public Nullable<decimal> Hours { get; set; }
        public Nullable<decimal> Amount { get; set; }
        [Display(Name = "Job Title")]
        public String JobTitle { get; set; }
        [Display(Name = "Project Type")]
        public int ProjectTypeId { get; set; }
        public List<SelectListItem> Jobs { get; set; }
        public JobViewModel Job { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Created By")]
        public string CreatedByName { get; set; }
        public String ClientName { get; set; }
        public List<SelectListItem> Clients { get; set; }
        public List<string> ClientsIds { get; set; }
        public decimal JobPrice { get; set; }
    }
}