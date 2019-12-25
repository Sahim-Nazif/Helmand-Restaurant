using Helmand.Models;
using System;
using Xunit;

namespace StaticDetails
{
    public class CartCountTest
    {
        [Fact]
        public void CanChangeCountValue()
        {

            //Arrange
            var count = new ShoppingCart();

            //Act
            var result=count.Count = 1;

            //Assert
            Assert.Equal(1, result);

        }
    }
}
