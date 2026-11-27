# Домашняя работа № 6

## Null, псевдонимы типов, статический импорт и наследование

**Учебная группа:** 251-337 (Рабочая группа Б)

**Рабочая группа:**

1. Мирошниченко Владимир Сергеевич
2. Астафьева Виола Александровна

**Лидер:**
Дмитриев Кирилл Денисович

---

## 1. Исходное задание

Продолжить проект маркетплейса. В проекте используются пространства имён `Marketplace.Domain` и `Marketplace.Services`; для каждого типа создаётся отдельный файл. Nullable-свойства помечаются знаком `?`, обязательные ссылочные свойства не помечаются и проверяются при необходимости. В файле проекта включается nullable-контекст:

```xml
<Nullable>enable</Nullable>
```

### Участник 1 — базовый `Product` и наследник `DigitalProduct`

Требуется дополнить `Product`:

- добавить псевдоним `using ProductId = System.Int32;`;
- добавить свойство `Id` типа `ProductId`;
- добавить nullable-свойство `Description`;
- добавить метод `GetDescription()`, возвращающий описание либо строку `"описание не указано"`;
- сохранить статический счётчик идентификаторов и обязательное свойство `Title`.

Требуется создать `DigitalProduct : Product`:

- свойства `DownloadUrl` и `FileSizeMb`;
- конструктор с null guard для `DownloadUrl` и проверкой размера файла;
- метод `GetFullInfo()`, вызывающий `base.GetDescription()`.

Проверяется, что `new DigitalProduct("url", 12) { Title = "Книга", Description = null }` компилируется; `new DigitalProduct(null!, 12) { Title = "Книга" }` выбрасывает `ArgumentNullException`; при `Description == null` метод `GetDescription()` возвращает строку по умолчанию; идентификатор наследника учитывается в общем `Product.CreatedCount`.

### Участник 2 — наследник `PhysicalProduct`

Требуется создать `PhysicalProduct : Product`:

- свойства `WeightKg` типа `double` и `WarehouseCode` типа `string?`;
- конструктор с явным вызовом `: base()`;
- проверку веса в приватном методе `EnsureWeight`;
- вычисляемое свойство `DeliveryInfo`, возвращающее значение склада или строку `"склад не назначен"`;
- метод `GetFullInfo()` с использованием `base.GetDescription()`.

Проверяется создание `new PhysicalProduct(1.5, null) { Title = "Стол" }`, значение `DeliveryInfo` для пустого склада, исключение при отрицательном весе, использование базового описания и увеличение общего счётчика `Product.CreatedCount`.

### Участник 3 — nullable-профиль покупателя

Требуется создать `CustomerProfile`:

- обязательное свойство `FullName`;
- nullable-свойства `MiddleName`, `Email`, `Phone`;
- свойство `Rating` типа `int` с проверкой диапазона от 0 до 5;
- псевдоним `using CustomerId = System.Int32;` и свойство `Id`;
- методы `GetDisplayName()`, `GetEmail()`, `EnsureEmail()` и `TryNormalizeEmail()`.

Требуется создать статический класс `ContactRules`:

- константа `UnknownEmail`;
- метод `NormalizeEmail(string? value)`;
- метод `LooksLikeEmail(string? value)`.

В `CustomerProfile` необходимо использовать статический импорт:

```csharp
using static Marketplace.Domain.ContactRules;
```

`GetDisplayName()` использует `MiddleName ?? string.Empty`; `GetEmail()` использует `Email ?? UnknownEmail`; `EnsureEmail()` использует `Email ??= UnknownEmail`; `TryNormalizeEmail()` вызывает `NormalizeEmail(Email)`. Проверяется создание покупателя с `Email = null`, строка по умолчанию для пустого email, заполнение только пустого email, нормализация строки и исключение при `Rating = 7`.

### Участник 4 — `PremiumCustomer` и `OrderService`

Требуется создать `PremiumCustomer : CustomerProfile`:

- свойство `DiscountPercent` типа `int`;
- конструктор `: base()` с проверкой скидки в диапазоне от 0 до 50;
- метод `GetPremiumInfo()`, использующий `base.GetDisplayName()`.

Требуется дополнить `OrderService`:

- `BuildGreeting(CustomerProfile? customer)` — возвращает имя покупателя или `"гость"`;
- `GetDiscount(CustomerProfile? customer)` — определяет `PremiumCustomer` через `is`;
- `GetEmail(CustomerProfile? customer)` — использует `?.` и `??`;
- `GetTotal(IEnumerable<CartItem>? items)` — выполняет null guard и получает сумму через `CheckoutService.GetTotal`;
- `FormatOrderNumber(OrderId id)` с псевдонимом `using OrderId = System.Int32;`;
- приватный метод `EnsureItems`.

Проверяется создание `PremiumCustomer`, скидка для премиального покупателя и для `null`, получение email для `null`, `ArgumentNullException` при `GetTotal(null)`, использование базового метода в `GetPremiumInfo()` и применение `OrderId` в `FormatOrderNumber()`.

### Компоновка

Точка входа должна:

- создавать `Product`, `DigitalProduct` и `PhysicalProduct`, выводить уникальные `Id` и `Product.CreatedCount`;
- показывать `Description ?? "описание не указано"` и `WarehouseCode ?? "склад не назначен"`;
- создавать обычного и премиального покупателя, демонстрировать `GetDisplayName`, `GetEmail`, `EnsureEmail`, `GetDiscount`;
- передавать `null` в `OrderService` и выводить безопасные результаты операторов `?.` и `??`;
- вызывать `GetTotal(null)` в блоке `try/catch` и показывать `ArgumentNullException`;
- демонстрировать порядок вызова конструкторов `Product` и `DigitalProduct`;
- демонстрировать псевдонимы `ProductId`, `CustomerId`, `OrderId` и статический импорт `ContactRules`.

---

## 2. Выполненное решение

### 2.1. Участник 1. `Product` и `DigitalProduct`

В базовый класс добавлены идентификатор `ProductId`, общий статический счётчик, обязательное название и nullable-описание. Метод `GetDescription()` использует оператор `??`, поэтому отсутствие описания не приводит к возврату `null`. Класс `DigitalProduct` наследует общие данные товара, проверяет URL через `ArgumentNullException.ThrowIfNull` и проверяет размер файла. В `GetFullInfo()` вызывается `base.GetDescription()`.

#### `Viva/Product.cs`

```csharp
using System.Globalization;
using ProductId = System.Int32;

namespace Marketplace.Domain;

public class Product
{
    private static int nextId;

    public static int CreatedCount => nextId;
    public ProductId Id { get; }
    public required string Title { get; init; }
    public string? Description { get; set; }

    public Product()
    {
        Id = checked(++nextId);
        Console.WriteLine($"  [Product] вызван базовый конструктор, Id = {Id}");
    }

    public string GetDescription() => Description ?? "описание не указано";
}
```

#### `Viva/DigitalProduct.cs`

```csharp
namespace Marketplace.Domain;

public class DigitalProduct : Product
{
    public string DownloadUrl { get; }
    public int FileSizeMb { get; }

    public DigitalProduct(string downloadUrl, int fileSizeMb)
    {
        ArgumentNullException.ThrowIfNull(downloadUrl);
        if (fileSizeMb <= 0)
            throw new ArgumentOutOfRangeException(nameof(fileSizeMb), "Размер файла должен быть больше нуля.");

        DownloadUrl = downloadUrl;
        FileSizeMb = fileSizeMb;
        Console.WriteLine("  [DigitalProduct] вызван конструктор производного класса");
    }

    public string GetFullInfo() =>
        $"{Title}: {base.GetDescription()}, размер файла: {FileSizeMb} МБ";
}
```

### 2.2. Участник 2. `PhysicalProduct`

Класс `PhysicalProduct` наследует `Product` и явно вызывает его конструктор через `: base()`. Вес проверяется до сохранения в свойстве: допускается только положительное конечное число. Nullable-код склада обрабатывается оператором `??`. Метод `GetFullInfo()` выводит данные базового и производного классов.

#### `Viva/PhysicalProduct.cs`

```csharp
namespace Marketplace.Domain;

public class PhysicalProduct : Product
{
    public double WeightKg { get; }
    public string? WarehouseCode { get; set; }

    public PhysicalProduct(double weightKg, string? warehouseCode) : base()
    {
        EnsureWeight(weightKg);
        WeightKg = weightKg;
        WarehouseCode = warehouseCode;
    }

    public string DeliveryInfo => WarehouseCode ?? "склад не назначен";

    public string GetFullInfo() =>
        $"{Title}: {base.GetDescription()}, вес: {WeightKg} кг, {DeliveryInfo}";

    private static void EnsureWeight(double weightKg)
    {
        if (!double.IsFinite(weightKg) || weightKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightKg), "Вес должен быть положительным конечным числом.");
    }
}
```

### 2.3. Участник 3. `ContactRules` и `CustomerProfile`

Класс `ContactRules` содержит правила работы с email. Нормализация удаляет внешние пробелы, приводит строку к нижнему регистру и возвращает значение по умолчанию для `null`. В `CustomerProfile` обязательным является только имя, а отчество, email и телефон могут отсутствовать. Статический импорт `ContactRules` позволяет обращаться к `UnknownEmail` и `NormalizeEmail` без указания имени класса.

#### `Viola/ContactRules.cs`

```csharp
namespace Marketplace.Domain;

public static class ContactRules
{
    public const string UnknownEmail = "почта не указана";

    public static string NormalizeEmail(string? value) =>
        value?.Trim().ToLowerInvariant() ?? UnknownEmail;

    public static bool LooksLikeEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        int atIndex = value.IndexOf('@');
        return atIndex > 0 && atIndex == value.LastIndexOf('@') && atIndex < value.Length - 1;
    }
}
```

#### `Viola/CustomerProfile.cs`

```csharp
using CustomerId = System.Int32;
using static Marketplace.Domain.ContactRules;

namespace Marketplace.Domain;

public class CustomerProfile
{
    public CustomerId Id { get; init; }
    public required string FullName { get; init; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    private int rating;
    public int Rating
    {
        get => rating;
        set
        {
            EnsureRating(value);
            rating = value;
        }
    }

    public string GetDisplayName() => $"{FullName} {MiddleName ?? string.Empty}".Trim();

    public string GetEmail() => Email ?? UnknownEmail;

    public void EnsureEmail()
    {
        Email ??= UnknownEmail;
    }

    public string TryNormalizeEmail() => NormalizeEmail(Email);

    private static void EnsureRating(int value)
    {
        if (value < 0 || value > 5)
            throw new ArgumentOutOfRangeException(nameof(value), "Рейтинг должен быть в диапазоне от 0 до 5.");
    }
}
```

### 2.4. Участник 4. `PremiumCustomer` и `OrderService`

Класс `PremiumCustomer` расширяет профиль покупателя скидкой и использует базовый метод для формирования имени. `OrderService` безопасно принимает nullable-покупателя: для отсутствующего объекта выводятся значения по умолчанию. Тип покупателя проверяется сопоставлением с образцом `is PremiumCustomer premium`. В `GetTotal()` сначала выполняется null guard, затем сумма передаётся на вычисление в `CheckoutService`.

#### `Viola/PremiumCustomer.cs`

```csharp
namespace Marketplace.Domain;

public class PremiumCustomer : CustomerProfile
{
    public int DiscountPercent { get; }

    public PremiumCustomer(int discountPercent) : base()
    {
        if (discountPercent < 0 || discountPercent > 50)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "Скидка должна быть в диапазоне от 0% до 50%.");

        DiscountPercent = discountPercent;
    }

    public string GetPremiumInfo() =>
        $"{base.GetDisplayName()} (Скидка: {DiscountPercent}%)";
}
```

#### `Viola/OrderService.cs`

```csharp
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
```

### 2.5. Вспомогательные типы

`CartItem` используется как позиция заказа, а `CheckoutService` вычисляет общую стоимость позиций. Эти типы необходимы для демонстрации требуемого метода `OrderService.GetTotal()`.

#### `Infrastructure/CartItem.cs`

```csharp
namespace Marketplace.Domain;

public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }
    public int Total => checked(Quantity * PricePerUnit);

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (pricePerUnit < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerUnit));

        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
```

#### `Infrastructure/CheckoutService.cs`

```csharp
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
```

### 2.6. Компоновка и точка входа

В точке входа создаются все требуемые виды товаров и покупателей. Программа выводит общий счётчик и идентификаторы, результаты обработки nullable-значений, скидку премиального покупателя и номер заказа. В блоках `try/catch` показаны `ArgumentNullException` и проверки диапазонов. Вывод в конструкторах `Product` и `DigitalProduct` демонстрирует, что базовый конструктор выполняется раньше конструктора наследника.

#### `Lead/Program.cs`

```csharp
using Marketplace.Domain;
using Marketplace.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Домашняя работа 6 — Marketplace");
Console.WriteLine("Null, псевдонимы типов, static import и наследование\n");

Console.WriteLine("1. Товары, наследование и общий счётчик Id");
var basic = new Product
{
    Title = "Базовый товар",
    Description = "Товар без специального типа"
};
var digital = new DigitalProduct("https://example.org/book.pdf", 12)
{
    Title = "Электронная книга",
    Description = null
};
var physical = new PhysicalProduct(1.5, null)
{
    Title = "Письменный стол"
};

Console.WriteLine($"  {basic.Title}: Id = {basic.Id}, {basic.GetDescription()}");
Console.WriteLine($"  {digital.GetFullInfo()}, Id = {digital.Id}");
Console.WriteLine($"  {physical.GetFullInfo()}, Id = {physical.Id}");
Console.WriteLine($"  Создано Product: {Product.CreatedCount}\n");

Console.WriteLine("2. Nullable-свойства профиля и static import ContactRules");
var customer = new CustomerProfile
{
    Id = 101,
    FullName = "Анна",
    MiddleName = null,
    Email = null,
    Phone = null,
    Rating = 5
};
Console.WriteLine($"  Имя: {customer.GetDisplayName()}");
Console.WriteLine($"  Email до EnsureEmail: {customer.GetEmail()}");
customer.EnsureEmail();
Console.WriteLine($"  Email после EnsureEmail: {customer.GetEmail()}");
customer.Email = "  Anna@EXAMPLE.RU ";
Console.WriteLine($"  Нормализованный email: {customer.TryNormalizeEmail()}");
Console.WriteLine($"  LooksLikeEmail: {ContactRules.LooksLikeEmail(customer.Email)}\n");

Console.WriteLine("3. PremiumCustomer и безопасная работа OrderService с null");
var premium = new PremiumCustomer(10)
{
    Id = 102,
    FullName = "Мария",
    MiddleName = "Игоревна",
    Email = "maria@example.ru"
};
var orderService = new OrderService();
Console.WriteLine($"  {premium.GetPremiumInfo()}");
Console.WriteLine($"  Скидка premium: {orderService.GetDiscount(premium)}%");
Console.WriteLine($"  Приветствие для null: {orderService.BuildGreeting(null)}");
Console.WriteLine($"  Email для null: {orderService.GetEmail(null)}");
Console.WriteLine($"  {orderService.FormatOrderNumber(501)}\n");

Console.WriteLine("4. Null guard в OrderService.GetTotal");
var items = new[]
{
    new CartItem(productId: basic.Id, quantity: 2, pricePerUnit: 150),
    new CartItem(productId: digital.Id, quantity: 1, pricePerUnit: 300)
};
Console.WriteLine($"  Сумма позиций: {orderService.GetTotal(items)} УЕ");
try
{
    orderService.GetTotal(null);
}
catch (ArgumentNullException exception)
{
    Console.WriteLine($"  Перехвачено: {exception.GetType().Name} ({exception.ParamName})");
}

Console.WriteLine("\n5. Проверка исключений");
try
{
    _ = new DigitalProduct(null!, 12) { Title = "Некорректный товар" };
}
catch (ArgumentNullException exception)
{
    Console.WriteLine($"  DownloadUrl == null: {exception.GetType().Name}");
}
try
{
    _ = new PhysicalProduct(-1, "A-1") { Title = "Некорректный товар" };
}
catch (ArgumentOutOfRangeException exception)
{
    Console.WriteLine($"  Некорректный вес: {exception.GetType().Name}");
}
try
{
    customer.Rating = 7;
}
catch (ArgumentOutOfRangeException exception)
{
    Console.WriteLine($"  Некорректный рейтинг: {exception.GetType().Name}");
}
```

#### `Homework_6.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

---

## 3. Таблица проекта

| Тип | Базовый тип | Nullable-члены | Используемые операторы и проверки | Исполнитель |
|---|---|---|---|---|
| `Product` | — | `Description` | `??`, `required` | Viva |
| `DigitalProduct` | `Product` | — | null guard, `!`, `base.GetDescription()` | Viva |
| `PhysicalProduct` | `Product` | `WarehouseCode` | `??`, `base()`, проверка диапазона | Viva |
| `ContactRules` | — | параметр `string? value` | `?.`, `??` | Viola |
| `CustomerProfile` | — | `MiddleName`, `Email`, `Phone` | `??`, `??=`, static import, проверка рейтинга | Viola |
| `PremiumCustomer` | `CustomerProfile` | — | `base()`, `base.GetDisplayName()`, проверка диапазона | Viola |
| `OrderService` | — | параметры `CustomerProfile?`, `IEnumerable<CartItem>?` | `?.`, `??`, `is`, null guard, `!` | Viola |
| `CartItem` | — | — | `checked`, проверка параметров | Lead |
| `CheckoutService` | — | — | null guard | Lead |
| `Program` | — | — | `try/catch`, демонстрация nullable-сценариев | Lead |

Псевдонимы типов используются в классах `Product` (`ProductId`), `CustomerProfile` (`CustomerId`) и `OrderService` (`OrderId`). Статический импорт `ContactRules` используется в `CustomerProfile`.

---

## 4. Результаты запуска

```text
Домашняя работа 6 — Marketplace
Null, псевдонимы типов, static import и наследование

1. Товары, наследование и общий счётчик Id
  [Product] вызван базовый конструктор, Id = 1
  [Product] вызван базовый конструктор, Id = 2
  [DigitalProduct] вызван конструктор производного класса
  [Product] вызван базовый конструктор, Id = 3
  Базовый товар: Id = 1, Товар без специального типа
  Электронная книга: описание не указано, размер файла: 12 МБ, Id = 2
  Письменный стол: описание не указано, вес: 1.5 кг, склад не назначен, Id = 3
  Создано Product: 3

2. Nullable-свойства профиля и static import ContactRules
  Имя: Анна
  Email до EnsureEmail: почта не указана
  Email после EnsureEmail: почта не указана
  Нормализованный email: anna@example.ru
  LooksLikeEmail: True

3. PremiumCustomer и безопасная работа OrderService с null
  Мария Игоревна (Скидка: 10%)
  Скидка premium: 10%
  Приветствие для null: гость
  Email для null: почта не указана
  Заказ №501

4. Null guard в OrderService.GetTotal
  Сумма позиций: 600 УЕ
  Перехвачено: ArgumentNullException (items)

5. Проверка исключений
  [Product] вызван базовый конструктор, Id = 4
  DownloadUrl == null: ArgumentNullException
  [Product] вызван базовый конструктор, Id = 5
  Некорректный вес: ArgumentOutOfRangeException
  Некорректный рейтинг: ArgumentOutOfRangeException
```

Вывод подтверждает порядок вызова конструкторов: при создании `DigitalProduct` сначала выполняется конструктор `Product`, затем конструктор `DigitalProduct`. Для `Description = null` и `WarehouseCode = null` используются заданные строки по умолчанию. Передача `null` в методы `OrderService`, где это предусмотрено контрактом, обрабатывается безопасно; передача `null` в `GetTotal()` приводит к `ArgumentNullException`.

---

## 5. Вопросы участников

Вопросов нету

---

## 6. Выводы

Nullable-свойства применены там, где отсутствие данных является допустимым состоянием: описание товара, код склада, отчество, email и телефон покупателя. Отсутствие URL файла, коллекции позиций или корректного числового значения является ошибкой входных данных, поэтому для этих случаев применены null guard и проверки диапазонов.

Наследники `DigitalProduct` и `PhysicalProduct` используют общие свойства товара и добавляют данные, характерные для своего типа. `PremiumCustomer` расширяет профиль покупателя скидкой, не дублируя хранение имени и контактов. Псевдонимы `ProductId`, `CustomerId` и `OrderId` делают назначение целочисленных идентификаторов понятнее, а статический импорт `ContactRules` упрощает работу с правилами email в `CustomerProfile`.

Проект собирается и запускается без ошибок и предупреждений.
