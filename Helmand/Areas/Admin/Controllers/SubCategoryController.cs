using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                    StatusMessage = "Error : Sub Category already exists under " + doesSubCategoryExists.First().Category.Name + " category.";
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

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "SubCId", "SubCName"));
        }

        //Get - Edit 
        [HttpGet]
        public async Task<IActionResult> EditSubcategory(int ? id)
        {
            //let's first check for id
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m =>m.SubCId == id);
            if (subCategory==null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCName).Select(p => p.SubCName).Distinct().ToListAsync(),
            };
            return View(model);
        }

        //Post method for Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditSubcategory(SubCategoryAndCategoryViewModel model)
        {
          
            //ModelState is a property of controller that is used for validating form in server side
            //for instance to check if inputs meets the validation
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.SubCName == model.SubCategory.SubCName && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error : Sub Category already exists under " + doesSubCategoryExists.First().Category.Name + " category.";
                }
                else
                {
                    //here we need to update the database with new subcategory name or item
                    var subCatFromDd = await _db.SubCategory.FindAsync(model.SubCategory.SubCId);
                    subCatFromDd.SubCName = model.SubCategory.SubCName;


                    //_db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            //now if the model state is now valid we will do the following
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCName).Select(p => p.SubCName).ToListAsync(),
                StatusMessage = StatusMessage
            };
           // modelVM.SubCategory.SubCId = id;

            return View(modelVM);
        }

        //Get- Delete action method

        [HttpGet]
        public async Task<IActionResult> DeleteSubCategory(int? id)
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.SubCName).Select(p => p.SubCName).Distinct().ToListAsync(),
            };

            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(model);

        }
        //Post-Delete Action method

        [HttpPost, ActionName("DeleteSubCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubCategoryConfirmed(int? id)
        {
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return View();
            }
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}