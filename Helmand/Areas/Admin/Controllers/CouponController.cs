using System;
using System.Collections.Generic;
using System.IO;
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
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;


        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        public IActionResult AddCoupon() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>AddCoupon(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                //first checking if model state is valid we will fetch the file that was uploaded for the image
                var files = HttpContext.Request.Form.Files;
                //if it is greater than zeros means the file is uploaded
                if(files.Count>0)
                {
                    //then will convert this to a stream of bytes to store it in the database
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupon.Picture = p1;
                }
               
                _db.Coupon.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
        //Editing Coupon

        public async Task<IActionResult>EditCoupon(int ?id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var coupon= await _db.Coupon.FirstOrDefaultAsync(c=>c.Id==id);
            if (coupon==null)
            {
                return NotFound();
            }
            return View(coupon);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditCoupon(Coupon coupon)
        {

            if (coupon.Id == 0)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupon.Where(c => c.Id == coupon.Id).FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                //if it is greater than zeros means the file is uploaded
                if (files.Count > 0)
                {
                    //then will convert this to a stream of bytes to store it in the database
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDB.Picture = p1;
                }
                couponFromDB.Name = coupon.Name;
                couponFromDB.MinimumAmount = coupon.MinimumAmount;
                couponFromDB.Discount = coupon.Discount;
                couponFromDB.CouponType = coupon.CouponType;
                couponFromDB.IsActive = coupon.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(coupon);
        }

        //Delete get

        public async Task<IActionResult> DeleteCoupon(int ?id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var getCoupon = await _db.Coupon.FirstOrDefaultAsync(c => c.Id == id);

            if (getCoupon == null)
            {
                return NotFound();
            }
            return View (getCoupon);
        }

        //Delete Post 
        [HttpPost, ActionName("DeleteCoupon")]
        public async Task<IActionResult> DeleteCouponPost(int ? id)
        {
            var getCoupons = await _db.Coupon.FindAsync(id);
            if (getCoupons==null)
            {
                return View();
            }


            _db.Coupon.Remove(getCoupons);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}