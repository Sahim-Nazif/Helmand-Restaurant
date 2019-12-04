using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Helmand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        //whenever we need to save anything on the server or we need the rout path of the app we need Webhosting
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}