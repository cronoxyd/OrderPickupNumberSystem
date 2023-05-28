using System.Diagnostics;

namespace OrderNumberPickupSystem;

public class OrderPickupNumberSystem : IOrderPickupNumberSystem
{
    public int MinOrderNumber { get; set; } = 1;

    public TimeSpan OrderNumberCooldown { get; set; } = TimeSpan.FromMinutes(10);

    protected Dictionary<int, OrderNumberInfo> OrderNumberDatabase { get; } = new();

    protected void RefreshOrderNumberCooldown()
    {
        foreach(var orderNumber in OrderNumberDatabase.Keys)
        {
            var numberInfo = OrderNumberDatabase[orderNumber];

            if (numberInfo.State != OrderNumberState.Cooldown)
                continue;

            if (numberInfo.CooldownStart == null)
                throw new InvalidDataException($"Order number {orderNumber} is in cooldown but the cooldown start has not been set");

            if ((DateTime.Now - numberInfo.CooldownStart) >= OrderNumberCooldown)
            {
                numberInfo.State = OrderNumberState.Idle;
                numberInfo.CooldownStart = null;
            }
        }
    }

    public int GetNextIdleOrderNumber()
    {
        RefreshOrderNumberCooldown();

        // Try and find an idle number
        foreach(var orderNumber in OrderNumberDatabase.Keys)
        {
            var numberInfo = OrderNumberDatabase[orderNumber];

            if (numberInfo.State != OrderNumberState.Idle)
                continue;

            if (orderNumber < MinOrderNumber)
                continue;

            numberInfo.State = OrderNumberState.InPreparation;
            Debug.WriteLine($"Found existing number {orderNumber} in {nameof(OrderNumberState.Idle)} state");
            return orderNumber;
        }

        // If there is no idle number, determine a new number to add to the
        // database and return

        // By default, use the minimum number
        int nextNumber = MinOrderNumber;

        // If there are entries in the database, find the next number based on
        // the highest existing number
        if (OrderNumberDatabase.Count > 0)
            nextNumber = Math.Max(nextNumber, OrderNumberDatabase.Keys.Max() + 1);

        // Use Add() method to force exception if the number already exists
        OrderNumberDatabase.Add(nextNumber, new() {
            State = OrderNumberState.InPreparation
        });

        Debug.WriteLine($"Found no existing number in {nameof(OrderNumberState.Idle)} state, created new number {nextNumber}");
        return nextNumber;
    }

    public int GetOrderNumberPoolSize() =>
        OrderNumberDatabase.Count;

    public int[] GetOrderNumbersInPreparation() =>
        OrderNumberDatabase
            .Where(kvp => kvp.Value.State == OrderNumberState.InPreparation)
            .Select(kvp => kvp.Key)
            .ToArray();

    public int[] GetOrderNumbersReadyForPickup() =>
        OrderNumberDatabase
            .Where(kvp => kvp.Value.State == OrderNumberState.ReadyForPickup)
            .Select(kvp => kvp.Key)
            .ToArray();

    public void SetOrderReady(int orderNumber)
    {
        var numberInfo = OrderNumberDatabase[orderNumber];

        if (numberInfo.State != OrderNumberState.InPreparation)
            throw new InvalidOperationException($"Order number {orderNumber} cannot be marked ready for pickup as it is not currently being prepared");

        numberInfo.State = OrderNumberState.ReadyForPickup;
    }

    public void SetOrderPickedUp(int orderNumber)
    {
        var numberInfo = OrderNumberDatabase[orderNumber];

        if (numberInfo.State != OrderNumberState.ReadyForPickup)
            throw new InvalidOperationException($"Order number {orderNumber} cannot be marked picked up it is not currently ready for pickup");

        numberInfo.State = OrderNumberState.Cooldown;
        numberInfo.CooldownStart = DateTime.Now;
    }
}
