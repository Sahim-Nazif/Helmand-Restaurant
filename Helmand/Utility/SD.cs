using Helmand.Models;
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

        public const string ssCouponCode = "ssCouponCode";

        //order status
        public const string StatuSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusRead= "Ready for Pickup";
        public const string StatuCompleted = "Completed";
        public const string StatuCancelled = "Cancelled";

        public const string PaymentStatusPending= "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
      



        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        public static double DiscountedPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
            if(couponFromDb==null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimumAmount>OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
                        //$10 off $100
                        return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);

                    }
                    
                        if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
                        {
                            //10% off $100
                            return Math.Round(OriginalOrderTotal - (OriginalOrderTotal* couponFromDb.Discount/100), 2);


                        }
                    }
            }
            return OriginalOrderTotal;
        }
    }
}
