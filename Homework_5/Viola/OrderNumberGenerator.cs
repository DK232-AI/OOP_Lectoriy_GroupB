using System;

namespace Marketplace.Services;

/// <summary>
/// Статический генератор уникальных номеров заказов.
/// Демонстрирует: static класс, private static поле, static конструктор.
/// </summary>
public static class OrderNumberGenerator
{
    private static int lastNumber;

    static OrderNumberGenerator()
    {
        lastNumber = 1000;
    }

    public static int LastNumber => lastNumber;

    public static int Next()
    {
        return checked(++lastNumber);
    }
}
