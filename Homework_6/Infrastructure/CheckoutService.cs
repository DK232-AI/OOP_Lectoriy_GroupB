using Marketplace.Domain;

namespace Marketplace.Services;

public class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return items.Sum(item => item.Total);
    }
}
