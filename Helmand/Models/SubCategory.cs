using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCId { get; set; }

        [Display(Name=("Subcategory Name"))]
        [Required]
        public string SubCName { get; set; }
        [Required]
        [Display(Name="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }


    }
}
