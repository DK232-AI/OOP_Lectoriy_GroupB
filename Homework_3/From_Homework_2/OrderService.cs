using System;
using System.Collections.Generic;

namespace Marketplace.Services;

using Marketplace.Domain;

/// <summary>
/// Сервис оплаты покупок. Оплата бонусами и деньгами выполняется в одном вызове метода Pay.
/// </summary>
public class OrderService
{
    /// <summary>
    /// Оплата покупки: часть итоговой цены закрывается бонусами в пределах разрешённой доли, остаток - деньгами с личного баланса.
    /// При нехватке средств состояние баланса не меняется.
    /// </summary>
    public PaymentResult Pay(BalanceState state, CalculatedPrice price)
    {
        decimal finalPrice = price.FinalPrice;

        // Списание в один вызов: сначала решаем, сколько закрывают бонусы и сколько деньги.
        (decimal bonuses, decimal money) = SplitPayment(price, state);

        // Итоговая цена не может быть закрыта деньгами полностью - отказ без списаний.
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

    /// <summary>
    /// Полный сценарий покупки: расчёт цены по персональной скидке, оплата, при успехе - обновление баланса, добавление покупок в историю и очистка корзины.
    /// При отказе корзина и баланс остаются без изменений.
    /// </summary>
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
                Message = "Корзина пуста - оплачивать нечего.",
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

    /// <summary>
    /// Делит итоговую цену на бонусную и денежную части.
    /// Бонусами можно закрыть не больше разрешённой доли и не больше остатка бонусов.
    /// </summary>
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
