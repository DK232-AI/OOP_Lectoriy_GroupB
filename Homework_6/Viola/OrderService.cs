using System;
using System.Collections.Generic;
using OrderId = System.Int32;
using Marketplace.Domain;

namespace Marketplace.Services;
public class OrderService
{
    /// <summary>
    /// Оставлены реализованные ранее методы OrderService, 
    /// далее добавлены новые по текущему заданию
    /// </summary>
    /// 
    public PaymentResult Pay(BalanceState state, CalculatedPrice price)
    {
        decimal finalPrice = price.FinalPrice;

        // Списание в один вызов: сначала решаем, сколько закрывают бонусы и сколько деньги.
        (decimal bonuses, decimal money) = SplitPayment(price, state);

        // Итоговая цена не может быть закрыта деньгами полностью — отказ без списаний.
        if (money > ToDecimal(state.Money))
            return Reject("Недостаточно средств на балансе: не хватает "
                          + (money - ToDecimal(state.Money)).ToString("F2") + " УЕ.", price, state);

        // Успешная оплата: новое состояние собирается выражением with,
        // поэтому исходная структура остаётся неизменной.
        BalanceState newState = state with
        {
            Money = state.Money - (double)money,
            Bonuses = state.Bonuses - (double)bonuses
        };

        string message = bonuses > 0m
            ? $"Оплата прошла успешно. Списано бонусов: {bonuses:F2}, денег: {money:F2} УЕ."
            : $"Оплата прошла успешно. Списано денег: {money:F2} УЕ, бонусы не использовались.";

        return new PaymentResult
        {
            Success = true,
            Message = message,
            BonusesSpent = bonuses,
            MoneySpent = money,
            NewBalance = newState,
            Price = price
        };
    }
    public PaymentResult Checkout(UserProfile profile, UserBasket basket, IReadOnlyCollection<Product> catalog)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        if (basket == null)
            throw new ArgumentNullException(nameof(basket));

        if (basket.IsEmpty)
            return new PaymentResult
            {
                Success = false,
                Message = "Корзина пуста — оплачивать нечего.",
                NewBalance = BuildBalanceState(profile),
                Price = new CalculatedPrice(0m, ToMultiplier(profile))
            };

        // Корзина сама считает расчётную цену с учётом персональной скидки покупателя.
        CalculatedPrice price = basket.CalculatePrice(catalog, ToMultiplier(profile));

        PaymentResult result = Pay(BuildBalanceState(profile), price);

        if (!result.Success)
            return result;

        profile.ApplyNewBalance(result.NewBalance);
        profile.AddPurchases(basket.GetPositions());
        basket.Clear();

        return result;
    }

    // Новые методы из задания участника 4 др 6

    public string BuildGreeting(CustomerProfile? customer)
    {
        return customer?.FullName ?? "гость";
    }

    public int GetDiscount(CustomerProfile? customer)
    {
        return customer is PremiumCustomer premium ? premium.DiscountPercent : 0;
    }

    public string GetEmail(CustomerProfile? customer)
    {
        return customer?.Email ?? "почта не указана";
    }

    public int GetTotal(IEnumerable<CartItem>? items)
    {
        EnsureItems(items);

        // Создаем экземпляр сервиса и вызываем его метод
        var checkoutService = new CheckoutService();
        return checkoutService.GetTotal(items!);
    }

    public string FormatOrderNumber(OrderId id) => $"Заказ №{id}";

    private static void EnsureItems(IEnumerable<CartItem>? items)
    {
        ArgumentNullException.ThrowIfNull(items);
    }

    private static (decimal bonuses, decimal money) SplitPayment(CalculatedPrice price, BalanceState state)
    {
        decimal finalPrice = price.FinalPrice;

        if (finalPrice <= 0m)
            return (0m, 0m);

        decimal bonusLimit = Math.Round(finalPrice * ToDecimal(state.BonusPaymentRatio), 2, MidpointRounding.AwayFromZero);

        // Нельзя потратить больше, чем есть на бонусном балансе.
        decimal bonuses = Math.Min(bonusLimit, Math.Round(ToDecimal(state.Bonuses), 2, MidpointRounding.AwayFromZero));

        if (bonuses < 0m)
            bonuses = 0m;

        decimal money = finalPrice - bonuses;
        return (bonuses, money);
    }

    private static PaymentResult Reject(string reason, CalculatedPrice price, BalanceState state) =>
        new()
        {
            Success = false,
            Message = reason,
            BonusesSpent = 0m,
            MoneySpent = 0m,
            NewBalance = state,
            Price = price
        };

    private static BalanceState BuildBalanceState(UserProfile profile) =>
        new(profile.BonusBalance, profile.MoneyBalance, profile.AllowedBonusRatio);

    /// <summary>Персональная скидка хранится как double, для расчёта приводится к decimal.</summary>
    private static decimal ToMultiplier(UserProfile profile) => (decimal)profile.PersonalDiscount;

    private static decimal ToDecimal(double value) => (decimal)value;
}
