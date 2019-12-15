using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0, Dollar=1 }

        [Required]
        [DataType(DataType.Currency)]
        public double Discount { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public double MinimumAmount { get; set; }
        //this time we will upload image to the database, no on the server
        public byte[] Picture { get; set; }

        public bool IsActive { get; set; }

    }
}
