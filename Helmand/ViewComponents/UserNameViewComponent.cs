using Helmand.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        //first will have create object our DB in order to get the user who is logged in
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {

        }
    }
}
