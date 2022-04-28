using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Models
{
    public class ClientViewModel:BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Skype { get; set; }
        [Display(Name="Email")]
        public string EmailAdress { get; set; }
        [Display(Name = "Created On")]
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Created By")]
        public string CreatedByName { get; set; }
    }
}