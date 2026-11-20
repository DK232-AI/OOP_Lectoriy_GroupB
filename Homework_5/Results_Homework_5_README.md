# Домашнее задание 5 — Marketplace

## Титульная часть

- Номер занятия: 5.
- Тема: статические поля, свойства, методы, конструкторы и классы; `const`, `static readonly`, `readonly`, `readonly struct`.

| Участник | Роль | Папка | Зона ответственности |
|---|---|---|---|
| `Дмитриев Кирилл Денисович` | **Лидер группы** | `Lead/` | Компоновка проекта, точка входа, отчёт |
| `Мирошниченко Владимир Сергеевич` | Участник | `Viva/` | Участник 1 — `Product`, Участник 2 — `PriceRules` |
| `Астафьева Виола Александровна` | Участник | `Viola/` | Участник 3 — `Money`, Участник 4 — `OrderNumberGenerator`, `ReceiptService` |

## Постановка задания

Работа продолжает тему маркетплейса, но данная сдаваемая сборка **обособлена**: исходники из прежних домашних работ не подключены. Один тип — один файл; доменные типы размещены в `Marketplace.Domain`, сервисы — в `Marketplace.Services`. Имена статических членов — с заглавной буквы, закрытых статических полей — со строчной.

- Участник 1 (`Product`): `const int MaxPrice = 1_000_000`; общий закрытый счётчик `nextId`, открытое свойство чтения `CreatedCount`; ID и время создания с доступом только для чтения; `static readonly CatalogStartedAt` и статический конструктор; сохранить свойства и статус из прошлого занятия. Проверки: три разных ID, общий счётчик 3, защита ID и времени создания от изменения, отклонение цены вне диапазона.
- Участник 2 (`PriceRules`): статический класс с `VatRate = 0.2m`, `MaxDiscountPercent = 70`, `static readonly DefaultCurrency = "RUB"`; статические `WithVat(int)` и `ApplyDiscount(int,int)` с приватными проверками; нельзя создавать экземпляр класса. Контрольные расчёты: `WithVat(1000) = 1200`, `ApplyDiscount(1000,10) = 900`; недопустимые аргументы отклоняются.
- Участник 3 (`Money`): `readonly struct`, свойства `Amount` и `Currency` только для чтения, `const MaxAmount = 10_000_000m`, `static readonly Zero`; конструктор с проверками, фабрика `FromRubles`, `Add` с проверкой валют. Проверки: `Zero`, сумма 1000+2000=3000, невозможность изменить `Amount`, отрицательной суммы, пустой валюты и сложения разных валют.
- Участник 4 (`OrderNumberGenerator`, `ReceiptService`): статический генератор с приватным счётчиком, статическим конструктором на 1000, свойством только для чтения и `Next()`; статический чек с номером, строками позиций, итогом и НДС, двумя приватными методами `PrepareLine` и `FormatMoney`; использование `CheckoutService.GetTotal`, `PriceRules` и генератора. Экземпляры статических классов создавать нельзя.
- Задача лидера: объединить типы в один проект, создать 3 товара и вывести ID и счётчик; вывести константы и время старта каталога; продемонстрировать `Money.Add`, `PriceRules.WithVat` и `ApplyDiscount`, собрать `List<CartItem>` и напечатать чек; в `try/catch` показать ошибки цены, скидки и разных валют; показать запреты на изменение ID, счётчика и `Money.Amount`.

Правила реализации: `const` для заранее известного числа/строки, `static readonly` для значения, задаваемого при запуске, `readonly`-поле задавать лишь при создании; структура `Money` остаётся неизменяемой, изменение возможно лишь созданием нового значения. Статические сервисы не хранят состояние отдельного покупателя.

## Структура и автономность

```text
Homework_5/
├── Homework_5.csproj
├── bin
├── obj
├── Results_Homework_5_README.md
├── Viva/
│   ├── Product.cs
│   └── PriceRules.cs
├── Viola/
│   ├── Money.cs
│   ├── OrderNumberGenerator.cs
│   ├── ReceiptService.cs
└── Lead/
    ├── Program.cs
    ├── CartItem.cs          # минимальная позиция для чека в этом проекте
    ├── Cart.cs              # самостоятельная корзина
    └── CheckoutService.cs   # GetTotal для формирования чека
```

## Проверка кода участников и решения

**Viva / Участник 1.** Первоначальный `Product` удовлетворял требованиям к `const`, счётчику, ID и защите цены. В исходном листинге `CatalogStartedAt` корректно инициализировалось в явном статическом конструкторе; при компоновке этот вариант сохранён. Дополнительно артикул товара, созданного конструктором без параметров, инициализируется из ID, чтобы он не оставался пустым.

**Viva / Участник 2.** `PriceRules` — действительно статический класс, контрольные расчёты совпадают с требуемыми. Проверки отвергают отрицательную цену и процент выше 70. Результаты преобразуются в `int` с отбрасыванием дробной части — это выбранное соглашение для учебной модели с целочисленной ценой.

**Viola / Участник 3.** `Money` имеет get-only свойства и не меняет исходное значение при `Add`. Валюты сравниваются строго, некорректная сумма и код валюты отклоняются; `Zero` и фабрика возвращают RUB. `checked` добавлен при суммировании для явной защиты от переполнения, далее конструктор проверяет максимальную сумму.

**Viola / Участник 4.** Генератор нумерует заказы, начиная с 1001; счётчик закрыт и доступен для чтения через `LastNumber`. В `ReceiptService` найден сценарий двойного обхода входного `IEnumerable`: изначально строки чека печатались отдельно от подсчёта итогов, что опасно для однократного источника. Список теперь материализуется один раз, сумма вычисляется до выдачи номера, а `FormatMoney` использует `PriceRules.DefaultCurrency`.

**Лидер.** `Program.cs` демонстрирует создание товаров, неизменность данных, статические правила, номера заказов, чек и исключения. Для самостоятельности добавлены минимальные `Cart`, `CartItem` и `CheckoutService`: прежние проекты в исходниках и ссылках сборки отсутствуют.

## Таблица членов проекта

| Член | Тип / вид | Назначение | Автор |
|---|---|---|---|
| `Product.MaxPrice` | `const int` | допустимый предел цены | Viva |
| `Product.nextId` | `private static int` | общий счётчик товаров | Viva |
| `Product.CreatedCount` | `static` get-only | чтение общего счётчика | Viva |
| `Product.CatalogStartedAt` | `static readonly DateTime` | единый момент инициализации класса | Viva |
| `Product.createdAt` | `private readonly DateTime` | время создания конкретного товара | Viva |
| `PriceRules.VatRate` | `const decimal` | ставка НДС | Viva |
| `PriceRules.MaxDiscountPercent` | `const int` | верхняя граница скидки | Viva |
| `PriceRules.DefaultCurrency` | `static readonly string` | валюта чека | Viva |
| `PriceRules.WithVat`, `ApplyDiscount` | `static` методы | вычисления без экземпляра | Viva |
| `Money.MaxAmount` | `const decimal` | предел суммы | Viola |
| `Money.Zero` | `static readonly Money` | общее нулевое значение RUB | Viola |
| `Money` | `readonly struct` | неизменяемые Amount и Currency | Viola |
| `OrderNumberGenerator.lastNumber` | `private static int` | счётчик заказов | Viola |
| `OrderNumberGenerator.LastNumber` | `static` get-only | чтение счётчика | Viola |
| `OrderNumberGenerator.Next()` | `static` метод | выдача следующего номера | Viola |
| `ReceiptService.BuildReceipt` | `static` метод | текст чека без объекта сервиса | Viola |

## Доказательства и результаты

Сборка на .NET 8: **0 ошибок и 0 предупреждений**. Программа показала три уникальных ID (1, 2, 3) и `Product.CreatedCount = 3`; `PriceRules.WithVat(1000) = 1200`, `PriceRules.ApplyDiscount(1000,15) = 850`. Первые прямые вызовы `OrderNumberGenerator.Next()` вернули 1001 и 1002; формирование чека выдало номер 1003, базовую сумму 2100 RUB, итог 2520 RUB и НДС 420 RUB. Проверены отрицательная и чрезмерная цена, скидка 71%, отрицательная цена для `PriceRules` и сложение RUB с USD: операции отвергнуты ожидаемыми исключениями. `Money.FromRubles(1000).Add(Money.FromRubles(500))` вернул 1500 RUB, оставив исходные 1000 RUB неизменными.

Компилятор отдельно проверен временным файлом, который не входит в архив: `product.Id = 10` и `product.CreatedAt = ...` дали **CS0200**; `Product.MaxPrice = 10` — **CS0131**; `Product.CatalogStartedAt = ...` — **CS0198**; `money.Amount = 2m` — **CS0200**. Отрицательная цена не является ошибкой компиляции: она выбрасывает `ArgumentOutOfRangeException` при запуске. Граничные проверки выполнялись отдельно: **17/17 успешно**, в том числе сумма и НДС в чеке, защита от `default(CartItem)` и работа с перечислением-итератором.

## Выводы

Общий счётчик товара и номеров заказов удобен как `static`: значение разделяется всеми экземплярами. При этом счётчик закрыт; его публичный сеттер позволил бы сбросить нумерацию и получить повторяющиеся идентификаторы. Правила НДС и предела скидки — константы; время запуска каталога — `static readonly`, так как его вычисляют при инициализации типа. Поле `createdAt` — экземплярное `readonly`, потому что разные товары создаются в разное время.

`Money` удобно сделать `readonly struct`: сумма с валютой — небольшое значение, а не сущность с общей идентичностью. `Add` создаёт новое значение; изменение одной переменной не портит другую. При этом `default(Money)` существует и имеет пустой код валюты без вызова конструктора, поэтому в реальных финансовых API нужно отдельно решить, допустимо ли такое значение. Для данного задания рекомендуемый ноль — проверенный `Money.Zero`.

## Листинги решений

Код вставлен непосредственно из файлов этого автономного проекта; один тип — один файл.

### `Viva/Product.cs`

```csharp
using System;
using System.Globalization;

namespace Marketplace.Domain;

/// <summary>
/// Товар с поддержкой статических данных: const, static readonly, static property.
/// Демонстрирует все разновидности статических членов.
/// </summary>
public class Product
{
    public const int MaxPrice = 1_000_000;

    private static int nextId;
    private static readonly string[] allowedStatuses = { "В наличии", "Нет в наличии", "Под заказ" };
    public static readonly DateTime CatalogStartedAt;

    // Явный статический конструктор выполняется CLR однократно до первого обращения к типу.
    static Product()
    {
        CatalogStartedAt = DateTime.UtcNow;
    }

    private readonly DateTime createdAt;
    private string article = string.Empty;
    private int price;

    public static int CreatedCount => nextId;
    public int Id { get; }
    public required string Title { get; init; }

    public string Article
    {
        get => article;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Артикул обязателен.", nameof(value));
            article = value.Trim();
        }
    }

    public int Price
    {
        get => price;
        set
        {
            if (value < 0 || value > MaxPrice)
                throw new ArgumentOutOfRangeException(nameof(value), $"Цена должна быть от 0 до {MaxPrice}.");
            price = value;
        }
    }

    public DateTime CreatedAt => createdAt;
    public bool IsBudget => Price < 25;
    public string Status { get; private set; } = "В наличии";

    public Product()
    {
        Id = checked(++nextId);
        createdAt = DateTime.Now;
        Article = Id.ToString(CultureInfo.InvariantCulture);
    }

    public void ChangeStatus(string newStatus)
    {
        EnsureStatus(newStatus);
        Status = newStatus;
    }

    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeIntProperty) => includeIntProperty
        ? $"{Title} [{Article}], цена: {Price} УЕ, статус: {Status}"
        : $"{Title} [{Article}], статус: {Status}";

    private static void EnsureStatus(string value)
    {
        if (Array.IndexOf(allowedStatuses, value) < 0)
            throw new ArgumentException("Недопустимый статус товара.", nameof(value));
    }
}
```

### `Viva/PriceRules.cs`

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Правила ценообразования: НДС и скидки.
/// Статический класс с const, static readonly и методами без состояния.
/// </summary>
public static class PriceRules
{
    public const decimal VatRate = 0.2m;
    public const int MaxDiscountPercent = 70;
    public static readonly string DefaultCurrency = "RUB";

    public static int WithVat(int price)
    {
        EnsurePrice(price);
        return checked((int)(price * (1m + VatRate)));
    }

    public static int ApplyDiscount(int price, int percent)
    {
        EnsurePrice(price);
        EnsurePercent(percent);
        return (int)(price * (100m - percent) / 100m);
    }

    private static void EnsurePrice(int price)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной.");
    }

    private static void EnsurePercent(int percent)
    {
        if (percent < 0 || percent > MaxDiscountPercent)
            throw new ArgumentOutOfRangeException(nameof(percent), $"Скидка должна быть от 0 до {MaxDiscountPercent} процентов.");
    }
}
```

### `Viola/Money.cs`

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Неизменяемая структура для суммы денег с валютой.
/// Readonly struct с const, static readonly, конструктором с проверками.
/// </summary>
public readonly struct Money
{
    public const decimal MaxAmount = 10_000_000m;
    public static readonly Money Zero = new(0m, "RUB");

    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        EnsureAmount(amount);
        EnsureCurrency(currency);

        Amount = amount;
        Currency = currency;
    }

    public static Money FromRubles(decimal amount) => new(amount, "RUB");

    public Money Add(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
            throw new InvalidOperationException($"Нельзя складывать суммы в разных валютах: {Currency} и {other.Currency}.");

        return new Money(checked(Amount + other.Amount), Currency);
    }

    public override string ToString() => $"{Amount:0.##} {Currency}";

    private static void EnsureAmount(decimal amount)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма не может быть отрицательной.");

        if (amount > MaxAmount)
            throw new ArgumentOutOfRangeException(nameof(amount), $"Сумма не может превышать {MaxAmount}.");
    }

    private static void EnsureCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Валюта не может быть пустой.", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Код валюты должен состоять из трёх заглавных символов.", nameof(currency));

        foreach (char c in currency)
        {
            if (c < 'A' || c > 'Z')
                throw new ArgumentException("Код валюты должен содержать только заглавные латинские буквы.", nameof(currency));
        }
    }
}
```

### `Viola/OrderNumberGenerator.cs`

```csharp
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
```

### `Viola/ReceiptService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>
/// Статический сервис для формирования текстового чека.
/// Демонстрирует: static класс без состояния, работа с другими сервисами.
/// </summary>
public static class ReceiptService
{
    public static string BuildReceipt(IEnumerable<CartItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        // Материализуем один раз: IEnumerable может быть одноразовым перечислением.
        var lines = items.ToList();
        var checkoutService = new CheckoutService();
        int totalWithoutVat = checkoutService.GetTotal(lines);
        int totalWithVat = PriceRules.WithVat(totalWithoutVat);

        var sb = new StringBuilder();
        int orderNumber = OrderNumberGenerator.Next();
        sb.AppendLine($"- Ваш чек по заказу №{orderNumber} -");
        foreach (var item in lines)
            sb.AppendLine(PrepareLine(item));

        sb.AppendLine(new string('-', 30));
        sb.AppendLine($"Итого с НДС: {FormatMoney(totalWithVat)}");
        sb.AppendLine($"В том числе НДС ({(PriceRules.VatRate * 100)}%): {FormatMoney(totalWithVat - totalWithoutVat)}");

        return sb.ToString();
    }

    private static string PrepareLine(CartItem item)
    {
        return $"- Товар #{item.ProductId} | {item.Quantity} шт. x {FormatMoney(item.PricePerUnit)} = {FormatMoney(item.Total)}";
    }

    private static string FormatMoney(int amount)
    {
        return $"{amount} {PriceRules.DefaultCurrency}";
    }
}
```

### `Viola/CartItem.cs`

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>Независимая позиция корзины для текущего задания.</summary>
public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }
    public int Total => checked(PricePerUnit * Quantity);

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (pricePerUnit < 0) throw new ArgumentOutOfRangeException(nameof(pricePerUnit));
        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
```

### `Viola/Cart.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>Собственная корзина текущего задания; не зависит от прошлых проектов.</summary>
public sealed class Cart
{
    private readonly List<CartItem> items = new();
    public IReadOnlyList<CartItem> Items => items.AsReadOnly();

    public void Add(CartItem item)
    {
        if (item.ProductId <= 0 || item.Quantity <= 0 || item.PricePerUnit < 0)
            throw new ArgumentException("Недопустимая позиция корзины.", nameof(item));
        items.Add(item);
    }
}
```

### `Viola/CheckoutService.cs`

```csharp
using System;
using System.Collections.Generic;

using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>Сервис без состояния, используемый при формировании чека.</summary>
public sealed class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        int total = 0;
        foreach (CartItem item in items)
        {
            if (item.ProductId <= 0 || item.Quantity <= 0 || item.PricePerUnit < 0)
                throw new ArgumentException("Недопустимая позиция корзины.", nameof(items));
            total = checked(total + item.Total);
        }
        return total;
    }
}
```

### `Lead/Program.cs`

```csharp
using System;
using System.Collections.Generic;
using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoProductStatics();
        DemoPriceRules();
        DemoMoney();
        DemoOrderNumberGenerator();
        DemoReceipt();
        DemoReadonly();
        DemoInvalidInputs();
    }

    private static void DemoProductStatics()
    {
        Section("1. Статические члены Product");

        Console.WriteLine($"Product.MaxPrice = {Product.MaxPrice} (const, с большой буквы)");
        Console.WriteLine($"Product.CatalogStartedAt = {Product.CatalogStartedAt} (static readonly DateTime)");
        Console.WriteLine();

        Console.WriteLine("Создаю товары:");
        var p1 = new Product { Title = "Кружка", Article = "A-001", Price = 450 };
        var p2 = new Product { Title = "Термос", Article = "A-002", Price = 1200 };
        var p3 = new Product { Title = "Стикеры", Article = "A-003", Price = 20 };

        Console.WriteLine($"  Товар 1: ID = {p1.Id}, CreatedAt = {p1.CreatedAt:O}");
        Console.WriteLine($"  Товар 2: ID = {p2.Id}");
        Console.WriteLine($"  Товар 3: ID = {p3.Id}");
        Console.WriteLine();

        Console.WriteLine($"Product.CreatedCount = {Product.CreatedCount} (static property)");
        Console.WriteLine($"Уникальные ID: {p1.Id != p2.Id && p2.Id != p3.Id && p1.Id != p3.Id}");
        Console.WriteLine();
    }

    private static void DemoPriceRules()
    {
        Section("2. Статический класс PriceRules");

        Console.WriteLine($"PriceRules.VatRate = {PriceRules.VatRate} (const decimal)");
        Console.WriteLine($"PriceRules.MaxDiscountPercent = {PriceRules.MaxDiscountPercent} (const int)");
        Console.WriteLine($"PriceRules.DefaultCurrency = \"{PriceRules.DefaultCurrency}\" (static readonly string)");
        Console.WriteLine();

        int price = 1000;
        int priceWithVat = PriceRules.WithVat(price);
        Console.WriteLine($"PriceRules.WithVat({price}) = {priceWithVat}");

        int discounted = PriceRules.ApplyDiscount(price, 15);
        Console.WriteLine($"PriceRules.ApplyDiscount({price}, 15) = {discounted}");
        Console.WriteLine();
    }

    private static void DemoMoney()
    {
        Section("3. Readonly struct Money");

        var m1 = new Money(1500m, "RUB");
        Console.WriteLine($"m1 = new Money(1500m, \"RUB\") → {m1}");

        var m2 = Money.FromRubles(2500m);
        Console.WriteLine($"m2 = Money.FromRubles(2500m) → {m2}");

        Console.WriteLine($"Money.Zero = {Money.Zero} (static readonly)");

        var m3 = m1.Add(m2);
        Console.WriteLine($"m1.Add(m2) → {m3}");
        Console.WriteLine();
    }

    private static void DemoOrderNumberGenerator()
    {
        Section("4. Статический класс OrderNumberGenerator");

        Console.WriteLine($"OrderNumberGenerator.LastNumber = {OrderNumberGenerator.LastNumber}");
        int order1 = OrderNumberGenerator.Next();
        int order2 = OrderNumberGenerator.Next();
        Console.WriteLine($"OrderNumberGenerator.Next() → {order1}");
        Console.WriteLine($"OrderNumberGenerator.Next() → {order2}");
        Console.WriteLine($"OrderNumberGenerator.LastNumber = {OrderNumberGenerator.LastNumber}");
        Console.WriteLine();
    }

    private static void DemoReceipt()
    {
        Section("5. Статический сервис ReceiptService");

        var cart = new Cart();
        cart.Add(new CartItem(1, 2, 450));
        cart.Add(new CartItem(2, 1, 1200));
        string receipt = ReceiptService.BuildReceipt(cart.Items);
        Console.WriteLine(receipt);
        Console.WriteLine($"Номер после чека: {OrderNumberGenerator.LastNumber}");
    }

    private static void DemoReadonly()
    {
        Section("6. Readonly поля и readonly struct");

        var product = new Product { Title = "Товар", Article = "TEST", Price = 100 };
        Console.WriteLine($"product.CreatedAt = {product.CreatedAt:O} (readonly поле)");

        var m = new Money(100m, "RUB");
        Console.WriteLine($"m = Money(100m, \"RUB\") — readonly struct");
        Console.WriteLine("  Все свойства get-only, изменение только через новую копию");
        // product.Id = 10;                 // CS0200: у свойства нет сеттера.
        // product.CreatedAt = DateTime.Now; // CS0200: у свойства нет сеттера.
        // product.Price = -1;               // ArgumentOutOfRangeException.
        // product.Price = 2_000_000;        // ArgumentOutOfRangeException.
        // Product.MaxPrice = 10;            // CS0131: присваивание константе.
        // Product.CatalogStartedAt = DateTime.UtcNow; // CS0198: static readonly.
        // m.Amount = 500m;                  // CS0200: свойство get-only в readonly struct.
        Console.WriteLine();
    }

    private static void DemoInvalidInputs()
    {
        Section("7. Проверки исключений и независимости значения Money");
        Try("Отрицательная цена", () => new Product { Title = "Ошибка", Price = -1 });
        Try("Цена выше MaxPrice", () => new Product { Title = "Ошибка", Price = Product.MaxPrice + 1 });
        Try("Скидка выше 70%", () => PriceRules.ApplyDiscount(1000, 71));
        Try("Отрицательная цена в PriceRules", () => PriceRules.WithVat(-1));
        Try("Разные валюты", () => new Money(10m, "RUB").Add(new Money(5m, "USD")));
        var original = Money.FromRubles(1000m);
        var copy = original.Add(Money.FromRubles(500m));
        Console.WriteLine($"Money: исходное {original}, новая сумма {copy}; исходное не меняется.");

        // Проверка ошибок компиляции (строки не должны быть активны):
        // product.Id = 42;                  // нет публичного сеттера.
        // Product.MaxPrice = 1;              // константе нельзя присваивать.
        // Product.CatalogStartedAt = DateTime.UtcNow; // static readonly.
        // original.Amount = 2m;              // свойство get-only.
    }

    private static void Try(string situation, Action action)
    {
        try { action(); Console.WriteLine($"ОШИБКА: {situation} принято"); }
        catch (ArgumentException ex) { Console.WriteLine($"Отклонено: {situation} — {ex.GetType().Name}"); }
        catch (InvalidOperationException ex) { Console.WriteLine($"Отклонено: {situation} — {ex.GetType().Name}"); }
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));
    }
}
```
