using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Helmand.Data;
using Helmand.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            //here we want to return a list of all the user, except the current user who is loged in
            //therefore we need to get the identity or user ID who is logged in. We can use ClaimsIdentity

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }
        //here we use string id since it's guid in db
        public async Task<IActionResult>Lock(string id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now.AddMinutes(10);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //here will unlock a user
        public async Task<IActionResult>Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}