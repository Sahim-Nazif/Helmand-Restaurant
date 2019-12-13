﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Admin.Controllers
{
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
                _db.Coupon.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}