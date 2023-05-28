namespace OrderNumberPickupSystem;

public interface IOrderPickupNumberSystem
{
    /// <summary>
    /// Configures the (inclusive) lower minimum order number.
    /// </summary>
    int MinOrderNumber { get; set; }

    TimeSpan OrderNumberCooldown { get; set; }

    /// <summary>
    /// Gets the next free order number and marks it as "in preparation"
    /// </summary>
    int GetNextIdleOrderNumber();

    /// <returns>
    /// All order number which are currently being prepared.
    /// </returns>
    int[] GetOrderNumbersInPreparation();

    /// <returns>
    /// All order number which are waiting for customer pickup.
    /// </returns>    
    int[] GetOrderNumbersReadyForPickup();

    /// <summary>
    /// Marks the order with the specified number as "ready for pickup".
    /// </summary>
    void SetOrderReady(int orderNumber);

    /// <summary>
    /// Gets the total number of order numbers, regardless of status.
    /// </summary>
    int GetOrderNumberPoolSize();

    /// <summary>
    /// Marks the order with the specified number as picked up and initiates the
    /// cooldown.
    /// </summary>
    void SetOrderPickedUp(int orderNumber);
}