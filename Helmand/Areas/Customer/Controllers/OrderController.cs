﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Models;
using Helmand.Models.ViewModels;
using Helmand.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        private int PageSize = 2;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult>Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                //the application here is the applicationUser 
                OrderHeader = await _db.OrderHeader.Include(o => o.Application).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails= await _db.OrderDetail.Where(o=>o.OrderId==id ).ToListAsync()

            };

            return View(orderDetailsViewModel);

        }
        public IActionResult Index()=>View();
        

        //order History
        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>(),

            };

            

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.Application).Where(u => u.UserId == claim.Value).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetail.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {

                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory? productPage=:"
            };


            return View(orderListVM);
        }


        [Authorize(Roles =SD.KitchenUser + " ," + SD.ManagerUser)]
        public async Task<IActionResult>ManageOrder()
        {

            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();



            List<OrderHeader> OrderHeaderList = await _db.OrderHeader
                .Where(o=>o.Status==SD.StatusSubmitted ||o.Status==SD.StatusInProcess)
                .OrderByDescending(o=>o.PickUpTime).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetail.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }

             return View(orderDetailsVM.OrderBy(o=>o.OrderHeader.PickUpTime));
        }

        //Retrieving order details
        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.FirstOrDefaultAsync(m => m.Id == Id),
                  OrderDetails = await _db.OrderDetail.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.Application = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }

        //order prepare action method
        [Authorize(Roles =SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();

            //Email logic to notify user that order is ready for pickup
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }



        //Order Pickup
        [Authorize]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail=null, string searchName = null, string searchPhone = null)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>(),

            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderHistory? productPage=:");

            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();

            if (searchName != null || searchEmail != null || searchPhone != null)
            {
                var user = new ApplicationUser();


                if (searchName != null)
                {
                    OrderHeaderList = await _db.OrderHeader.Include(o => o.Application)
                                             .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                                             .OrderByDescending(o => o.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();

                        OrderHeaderList = await _db.OrderHeader.Include(o => o.Application)
                                                 .Where(o => o.UserId == user.Id)
                                                 .OrderByDescending(o => o.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = await _db.OrderHeader.Include(o => o.Application)
                                                     .Where(u => u.PhoneNumber.Contains(searchName))
                                                     .OrderByDescending(o => o.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {

                OrderHeaderList = await _db.OrderHeader.Include(o => o.Application).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }
                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailsViewModel individual = new OrderDetailsViewModel
                    {
                        OrderHeader = item,
                        OrderDetails = await _db.OrderDetail.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    orderListVM.Orders.Add(individual);
                }


            //here will need the order status -- ready for pickup

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {

                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };


            return View(orderListVM);
        }

        [Authorize (Roles =SD.FrontDeskUser + "," + SD.ManagerUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            return RedirectToAction("OrderPickup", "Order");
        }
    }
}