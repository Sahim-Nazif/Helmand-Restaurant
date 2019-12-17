using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helmand.Utility
{
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        //different roles for the user to be able to use the application
        //these fields/properties will be accessed in Register Model
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser="Customer";
    }
}
