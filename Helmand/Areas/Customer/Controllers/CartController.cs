using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;

        }

        
        public async Task<IActionResult> Index()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };


            //in order to calculate the shopping cart total, we to set initialize it to zero

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);


            return View();
        }
    }
}