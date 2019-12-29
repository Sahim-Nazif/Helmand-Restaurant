using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult>Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                //the application here is the applicationUser 
                OrderHeader = await _db.OrderHeader.Include(o => o.Application).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails= await _db.OrderDetail.Where(o=>o.OrderId==id ).ToListAsync()

            };

            return View(orderDetailsViewModel);

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}