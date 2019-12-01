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
        [TempData]
        public string StatusMessage { get; set; }

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
        //Post method for creating or adding sub category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>AddSubCategory(SubCategoryAndCategoryViewModel model)
        {

            //ModelState is a property of controller that is used for validating form in server side
            //for instance to check if inputs meets the validation
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.SubCName == model.SubCategory.SubCName && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count()>0)
                {
                    StatusMessage = "Error : Sub Category exists under" + doesSubCategoryExists.First().Category.Name + " Please another name";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
           
      //now if the model state is now valid we will do the following
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory=model.SubCategory,
                SubCategoryList= await _db.SubCategory.OrderBy(p=>p.SubCName).Select(p=>p.SubCName).ToListAsync(),
                StatusMessage= StatusMessage
            };

            return View(modelVM);
        }
    }
}