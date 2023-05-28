namespace OrderNumberPickupSystem.Tests;

[TestClass]
public class OrderNumberPickupSystemTests
{
    [TestMethod]
    public void TestOrderNumberCooldown()
    {
        var orderNumberSystem = new OrderPickupNumberSystem()
        {
            OrderNumberCooldown = TimeSpan.FromMilliseconds(2000),
            MinOrderNumber = 1
        };

        // Get some order numbers
        Assert.AreEqual(1, orderNumberSystem.GetNextIdleOrderNumber());
        Assert.AreEqual(2, orderNumberSystem.GetNextIdleOrderNumber());
        Assert.AreEqual(3, orderNumberSystem.GetNextIdleOrderNumber());

        // Mark 3 as ready for pickup and then have it picked up
        orderNumberSystem.SetOrderReady(3);
        orderNumberSystem.SetOrderPickedUp(3);

        // Cooldown has started for number 3
        // Wait a second and then get a new number (which should still not be
        // number 3)
        Thread.Sleep(1000);
        Assert.AreEqual(4, orderNumberSystem.GetNextIdleOrderNumber());

        // Wait a bit more than a second and then get another number
        Thread.Sleep(1500);
        Assert.AreEqual(3, orderNumberSystem.GetNextIdleOrderNumber());

        // Get a final number
        Assert.AreEqual(5, orderNumberSystem.GetNextIdleOrderNumber());

        // Check the pool size of the numbers
        Assert.AreEqual(5, orderNumberSystem.GetOrderNumberPoolSize());
    }

    [TestMethod]
    public void TestOrderNumbersInPreparation()
    {
        var orderNumberSystem = new OrderPickupNumberSystem()
        {
            OrderNumberCooldown = TimeSpan.FromMilliseconds(2000),
            MinOrderNumber = 1
        };

        // Get some numbers and set one number as ready for pickup
        Assert.AreEqual(1, orderNumberSystem.GetNextIdleOrderNumber());
        Assert.AreEqual(2, orderNumberSystem.GetNextIdleOrderNumber());
        Assert.AreEqual(3, orderNumberSystem.GetNextIdleOrderNumber());
        orderNumberSystem.SetOrderReady(3);
        Assert.AreEqual(4, orderNumberSystem.GetNextIdleOrderNumber());

        // Get the list of numbers in preparation and test its contents
        var orderNumbersInPrep = orderNumberSystem.GetOrderNumbersInPreparation();
        Assert.AreEqual(3, orderNumbersInPrep.Length);
        CollectionAssert.AllItemsAreUnique(orderNumbersInPrep);
        CollectionAssert.Contains(orderNumbersInPrep, 1);
        CollectionAssert.Contains(orderNumbersInPrep, 2);
        CollectionAssert.Contains(orderNumbersInPrep, 4);

        // Check the pool size of the numbers
        Assert.AreEqual(4, orderNumberSystem.GetOrderNumberPoolSize());
    }

    [TestMethod]
    public void TestOrderNumberMin()
    {
        var orderNumberSystem = new OrderPickupNumberSystem()
        {
            OrderNumberCooldown = TimeSpan.FromMilliseconds(100),
            MinOrderNumber = 1
        };

        // Create some order numbers with the minimum of 1
        Assert.AreEqual(1, orderNumberSystem.GetNextIdleOrderNumber());
        Assert.AreEqual(2, orderNumberSystem.GetNextIdleOrderNumber());

        // Set the minimum higher than the existing numbers and get a new number
        orderNumberSystem.MinOrderNumber = 100;
        Assert.AreEqual(100, orderNumberSystem.GetNextIdleOrderNumber());

        // Simulate an order being prepared and picked up, then wait for the
        // timeout to expire
        orderNumberSystem.SetOrderReady(2);
        orderNumberSystem.SetOrderPickedUp(2);
        Thread.Sleep(500);

        // The number system may still not return the newly idle number due to 
        // the minimum number
        Assert.AreEqual(101, orderNumberSystem.GetNextIdleOrderNumber());

        // Reset the number minimum to 1 and then expect the now idle number
        // (from before)
        orderNumberSystem.MinOrderNumber = 1;
        Assert.AreEqual(2, orderNumberSystem.GetNextIdleOrderNumber());

        // Check the pool size of the numbers
        Assert.AreEqual(4, orderNumberSystem.GetOrderNumberPoolSize());
    }
}
