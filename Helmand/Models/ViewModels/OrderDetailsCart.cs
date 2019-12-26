using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public IEnumerable<ShoppingCart>listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
