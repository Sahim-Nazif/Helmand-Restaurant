using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Admin.Controllers
{
    [Authorize(Roles=SD.ManagerUser)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get Action Method to retrieve data from Database
        public async Task <IActionResult> Index() 
        {

            return View(await _db.Category.ToListAsync());

        }

        //the method to retreive the view for AddCategory page
        public IActionResult AddCategory()=> PartialView();
        

        //Post- Add-Category method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if(ModelState.IsValid)
            {
                //if state is valid we will add the data to database
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Get- Edit

        [HttpGet] 
        public async Task<IActionResult>EditCategory(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category==null)
            {
                return NotFound();
            }
            return View(category);

        }

        //posting method for Edit category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
            
        }
        //Get- Delete action method

        [HttpGet]
        public async Task<IActionResult>DeleteCategory(int ?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        //Post-Delete Action method

        [HttpPost,ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteConfirmed(int ? id)
        {
            var category = await _db.Category.FindAsync(id);
            if(category==null)
            {
                return View();
            }
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}