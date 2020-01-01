using Helmand.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Helmand.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        //first will have create object our DB in order to get the user who is logged in
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //will get the user identity 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == claim.Value);

            return View(userFromDb); 

        }
    }
}
