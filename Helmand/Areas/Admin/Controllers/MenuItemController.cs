using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        //whenever we need to save anything on the server or we need the rout path of the app we need Webhosting
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemView { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            MenuItemView = new MenuItemViewModel()
            {
                Category = _db.Category,
                //here we don't need to attach subcategory, since it will based on what category gets selected
                MenuItem=new Models.MenuItem()
            };

        }
        //Get-Action Method
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuitems = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync();
            return View(menuitems);
        }

        //Adding menu-item to db

        public IActionResult AddMenuItem()
        {
            return View(MenuItemView);
        }


    }
}