using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Models.ViewModels;
using Helmand.Utility;
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
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                //here we don't need to attach subcategory, since it will based on what category gets selected
                MenuItem = new Models.MenuItem()
            };

        }
        //Get-Action Method
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuitems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuitems);
        }

        //Adding menu-item to db

        public IActionResult AddMenuItem()
        {
            return View(MenuItemVM);
        }

        //Get-Post Action method for menut Item
        [HttpPost, ActionName("AddMenuItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuItemPost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }
            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();
            //here we have to work on how to save image

            string webRootPath = _webHostEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);
            if (files.Count > 0)
            {
                //files has been uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //if no file was uploaded
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";

            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Editing menu-item 

        public async Task<IActionResult> EditMenuItem(int ? id)
        {
          if (id==null)
            {
                return NotFound();
            }
          //here we will retrieve the menu item
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            if (MenuItemVM.MenuItem==null)
            {
                return NotFound();
            }

            
                return View(MenuItemVM);
          
        }

        //Get-Post Action method for menut Item
        [HttpPost, ActionName("EditMenuItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItemPost(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }
         
            //here we have to work on how to save image

            string webRootPath = _webHostEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);
            if (files.Count > 0)
            {
                //New Image has been uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var new_extension = Path.GetExtension(files[0].FileName);

                //delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                //we will upload the new file 
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + new_extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + new_extension;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Get-Delete action

        public async Task<IActionResult> DeleteMenuItem(int? id)
        {
           

            if (id== null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m=> m.SubCategory).SingleOrDefaultAsync(m=> m.Id == id);
            if (MenuItemVM.MenuItem==null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        //Post Delete Action method

            [HttpPost,ActionName("DeleteMenuItem")]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuItemPost(int id)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            MenuItem menuItem = await _db.MenuItem.FindAsync(id);

            if (menuItem != null)
            {
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.MenuItem.Remove(menuItem);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}