using Marketplace.Domain;
using OrderId = System.Int32;

namespace Marketplace.Services;

public class OrderService
{
    public string BuildGreeting(CustomerProfile? customer) => customer?.FullName ?? "гость";

    public int GetDiscount(CustomerProfile? customer) =>
        customer is PremiumCustomer premium ? premium.DiscountPercent : 0;

    public string GetEmail(CustomerProfile? customer) =>
        customer?.Email ?? "почта не указана";

    public int GetTotal(IEnumerable<CartItem>? items)
    {
        EnsureItems(items);
        return new CheckoutService().GetTotal(items!);
    }

    public string FormatOrderNumber(OrderId id) => $"Заказ №{id}";

    private static void EnsureItems(IEnumerable<CartItem>? items)
    {
        ArgumentNullException.ThrowIfNull(items);
    }
}
