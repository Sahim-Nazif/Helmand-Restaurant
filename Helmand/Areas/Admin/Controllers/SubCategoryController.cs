using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        //  //Get Action Method to retrieve data from Database
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }
    

        //the method to retreive the view for AddSubCategory page
        [HttpGet]
        public async Task<IActionResult> AddSubCategory()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCName).Select(p => p.SubCName).Distinct().ToListAsync(),
        };
        return View(model);
    }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>AddSubCategory(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.SubCName == model.SubCategory.SubCName && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count()>0)
                {
                    //errro
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory=model.SubCategory,
                SubCategoryList= await _db.SubCategory.OrderBy(p=>p.SubCName).Select(p=>p.SubCName).ToListAsync()
            };

            return View(modelVM);
        }
    }
}