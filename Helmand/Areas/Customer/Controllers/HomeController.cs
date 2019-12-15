using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Helmand.Models;
using Helmand.Data;
using Helmand.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        //Getting properties for the index view model
        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel
            {

               MenuItem= await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
               Category= await _db.Category.ToListAsync(),
               Coupon= await _db.Coupon.Where(c=>c.IsActive==true).ToListAsync()
        };
            return View(IndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
