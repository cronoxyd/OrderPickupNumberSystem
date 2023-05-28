namespace OrderNumberPickupSystem;

public class OrderNumberInfo
{
    public OrderNumberState State { get; set; } = OrderNumberState.Idle;

    public DateTime? CooldownStart { get; set; } = null;
}
