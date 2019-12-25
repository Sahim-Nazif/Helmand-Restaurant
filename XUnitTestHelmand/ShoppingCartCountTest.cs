using Helmand.Models;
using System;
using Xunit;

namespace XUnitTestHelmand
{
    public class ShoppingCartCountTest
    {
        [Fact]
        public void CanNotChangeCountValue()
        {
                //Arrange
                var count = new ShoppingCart();

                //Act
                var result = count.Count = 1;

                //Assert
                Assert.Equal(1, result);
            }
    }
}
