using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name="FirstName")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        [MaxLength(8)]
        public string PostalCode { get; set; }
        


    }
}
