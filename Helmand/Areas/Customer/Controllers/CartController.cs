﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models.ViewModels;
using Helmand.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;

        }
       
        public async Task<IActionResult> Index()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };


            //in order to calculate the shopping cart total, will initialize OrderTotal to zero

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                OrderDetailsCartVM.listCart = cart.ToList();
            }
            // to calculate the order total

            foreach (var list in OrderDetailsCartVM.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m=>m.Id==list.MenuItemId);

                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                 list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);

                if (list.MenuItem.Description.Length>100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            OrderDetailsCartVM.OrderHeader.OrderTotalOriginal = OrderDetailsCartVM.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, OrderDetailsCartVM.OrderHeader.OrderTotalOriginal);

            }

            return View(OrderDetailsCartVM);
        }
        public IActionResult AddCoupon()
        {
            if(OrderDetailsCartVM.OrderHeader.CouponCode==null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = "";

            }
            HttpContext.Session.SetString(SD.ssCouponCode, OrderDetailsCartVM.OrderHeader.CouponCode);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveCoupon()
        {
            
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }
    }
}