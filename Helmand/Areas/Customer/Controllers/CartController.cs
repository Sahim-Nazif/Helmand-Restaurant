using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Models.ViewModels;
using Helmand.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Helmand.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public CartController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;

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
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);

                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);

                if (list.MenuItem.Description.Length > 100)
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
        //Shopping Cart Summary
        public async Task<IActionResult> Summary()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };


            //in order to calculate the shopping cart total, will initialize OrderTotal to zero

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                OrderDetailsCartVM.listCart = cart.ToList();
            }
            // to calculate the order total

            foreach (var list in OrderDetailsCartVM.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);

                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
               
            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOriginal = OrderDetailsCartVM.OrderHeader.OrderTotal;

            OrderDetailsCartVM.OrderHeader.PickupName = applicationUser.FirstName;
            OrderDetailsCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            OrderDetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, OrderDetailsCartVM.OrderHeader.OrderTotalOriginal);

            }

            return View(OrderDetailsCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            //in order to calculate the shopping cart total, will initialize OrderTotal to zero


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            //here we need to modify the cart and here we get the shopping cart
            OrderDetailsCartVM.listCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            OrderDetailsCartVM.OrderHeader.PaymentStatus =SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate =DateTime.Now;
            OrderDetailsCartVM.OrderHeader.UserId = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.PickUpTime =Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + "  " +OrderDetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());


            //here will create a list of order details in DB
            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(OrderDetailsCartVM.OrderHeader);

            await _db.SaveChangesAsync();


            foreach (var item in OrderDetailsCartVM.listCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCartVM.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count=item.Count

                };
                OrderDetailsCartVM.OrderHeader.OrderTotalOriginal+= orderDetails.Count * orderDetails.Price;
                _db.OrderDetail.Add(orderDetails);
                
               
            }


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, OrderDetailsCartVM.OrderHeader.OrderTotalOriginal);

            }
            //here if no coupon was used
            else
            {
                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotalOriginal;
            }
            OrderDetailsCartVM.OrderHeader.CouponCodeDiscount = OrderDetailsCartVM.OrderHeader.OrderTotalOriginal - OrderDetailsCartVM.OrderHeader.OrderTotal;

     
            //here once everthing is added to DB, will have to remove these items from shopping cart and set session to zero
            _db.ShoppingCart.RemoveRange(OrderDetailsCartVM.listCart);
            HttpContext.Session.SetInt32("startSessionCartCount", 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID :" + OrderDetailsCartVM.OrderHeader.Id,
                Source = stripeToken

            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            if(charge.BalanceTransactionId==null)
            {
                OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusRejected;
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower()=="succeeded")
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            await _db.SaveChangesAsync();
            //return RedirectToAction("Index", "Home");

            return RedirectToAction("Confirm", "Order",new { id = OrderDetailsCartVM.OrderHeader.Id });
        }


        public IActionResult AddCoupon()
        {
            if (OrderDetailsCartVM.OrderHeader.CouponCode == null)
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

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                //here will remove from the session
                var count = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32("startSessionCartCount", count);

            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            //here will remove from the session
            var count = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("startSessionCartCount", count);

            return RedirectToAction(nameof(Index));

        }
 
    }
}