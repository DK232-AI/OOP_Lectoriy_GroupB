# ООП 2026 — Домашнее задание 4

## 1. Титульная часть

| Параметр | Значение |
|---|---|
| Номер занятия | 4 |
| Тема | Свойства: автоматические, полная запись, вычисляемые, `init`, `required`; модификаторы доступа в свойствах; свойства структур и классов; перегрузка методов |
| Вариант проекта | Marketplace — свойства и перегрузки |
| Состав рабочей группы | 3 человека |

**Состав рабочей группы 251-337 (Группа Б):**

| Участник | Роль | Папка | Зона ответственности |
|---|---|---|---|
| `Дмитриев Кирилл Денисович` | **Лидер группы** | `Lead/` | Компоновка проекта, точка входа, отчёт |
| `Мирошниченко Владимир Сергеевич` | Участник | `Viva/` | Участник 1 — `Product`, Участник 2 — `CustomerProfile` |
| `Астафьева Виола Александровна` | Участник | `Viola/` | Участник 3 — `Discount`, Участник 4 — `CartItem`, `CheckoutService` |

> ФИО вписать перед отправкой.

**Соответствие пунктам требований к отчёту:**

| Пункт требований | Раздел отчёта |
|---|---|
| 1. Титульная часть | Раздел 1 |
| 2. Текст исходного задания | Раздел 2 |
| 3. Краткое описание решения (2–5 предложений) | Раздел 4 (4.1–4.4) |
| 4. Листинг решения под каждым описанием | Раздел 4 (4.1–4.4) |
| 5. Таблица свойств проекта | Раздел 5 |
| 6. Доказательства: `GetInfo`, отрицательная цена, попытка изменить `init` | Раздел 6 |
| 7. Список вопросов от участников | необязательный, не приводится |
| 8. Выводы: свойство / открытое поле / метод, где перегрузка упростила вызов | Раздел 8 |

## 2. Текст исходного задания

### Темы занятия

- Определение свойств
- Свойства для чтения и записи
- Вычисляемые свойства
- Модификаторы доступа в свойствах
- Автоматические свойства
- Блок `init`
- Сокращённая запись свойств
- Модификатор `required`
- Свойства структур и классов
- Перегрузка методов

### Задание 3 (групповое)

#### Проект

Мы продолжаем проект маркетплейса (занятия 2 и 3). Каталог, доступ к данным и печать уже готовы; на этом занятии заменяем методы `Get...` / `Set...` на **свойства** и добавляем **перегрузки** `GetInfo`.

**Правила проекта:**

1. Один тип — один файл; пространства имён `Marketplace.Domain` и `Marketplace.Services`.
2. Имена свойств — существительные с большой буквы (`Price`, `IsBudget`), имена методов — глаголы (`Calculate`, `GetReport`).
3. Названия типов из занятий 2 и 3 не меняются: `Product`, `CustomerProfile`, `Cart`, `CartItem` (у `CartItem` меняется только вид: класс становится `readonly struct`).

#### Участник 1 — класс `Product` (`Product.cs`) — главный по теме занятия

Класс должен содержать все четыре вида свойств из лабораторной и две перегрузки `GetInfo`.

1. `string` — сокращённая (автоматическая) запись, обязательная при создании: `public required string Title { get; init; }`.
2. `string` — полная запись с проверкой на пустоту: резервное поле `article`, свойство `Article` с проверкой `IsNullOrWhiteSpace` и `throw ArgumentException`.
3. `int` — полная запись, не допускает отрицательных значений: резервное поле `price`, свойство `Price` с проверкой `value < 0`.
4. `bool` — вычисляемое свойство: `IsBudget` равно `true`, если `Price < 25`.
5. Перегрузки: `public string GetInfo()` и `public string GetInfo(bool includeIntProperty)`.
6. Свойство `Status` меняется только внутри класса: `public string Status { get; private set; } = "В наличии"`.

**Требуется дополнительно:**

1. Свойство `Status` с `private set` (статус меняется только внутри класса).
2. Метод `ChangeStatus(string newStatus)` с приватной проверкой допустимых значений: «В наличии», «Нет в наличии», «Под заказ» (список допустимых значений хранится в приватном статическом поле).
3. Убедиться, что `product.Price = -10;` и `product.Status = "Продан";` не компилируются, а `product.ChangeStatus("Продан");` выбрасывает исключение.

#### Участник 2 — класс `CustomerProfile` (`CustomerProfile.cs`)

**Требуется:**

1. `FullName` — автоматическое свойство, `required` + `init`.
2. `Phone` — автоматическое свойство с инициализатором `string.Empty`.
3. `Email` — полная запись: сеттер проверяет, что строка содержит `@` (приватный метод `EnsureValidEmail`).
4. `Rating` — `int`, полная запись с проверкой диапазона 0…5, резервное поле `rating`.
5. Вычисляемое свойство `IsTrusted => Rating >= 4`.
6. Перегруженные методы: `string GetInfo()`, `string GetInfo(bool includeRating)`, `string GetContactInfo(bool includePhone)`.
7. 1–2 приватных метода: `EnsureValidEmail`, `NormalizePhone`.

#### Участник 3 — скидки и расчёт (`Discount.cs`)

**Требуется:**

1. Класс `Discount` со свойствами:
   - `Name` — сокращённая запись, `required`;
   - `Percent` — `decimal`, полная запись с проверкой 0…100;
   - `MinSum` — `int`, полная запись с проверкой `>= 0` (минимальная сумма заказа);
   - `ValidUntil` — `DateTime` с `init`;
   - `IsActive` — вычисляемое `bool` (`ValidUntil > DateTime.Now`).
2. Метод `int Calculate(int total)` — цена после скидки с учётом минимальной суммы; внутри 1–2 приватных метода (`ApplyDiscount`, `EnsurePercent`).
3. Перегрузка `int Calculate(int total, decimal percent)`.
4. Показать, что передача недопустимых значений через свойства не проходит.

#### Участник 4 — позиция корзины и сервис оформления заказа (`CartItem.cs`, `CheckoutService.cs`)

**Требуется:**

1. `readonly struct CartItem` — позиция корзины со свойствами `ProductId`, `Quantity`, `PricePerUnit` — только для чтения, задаются конструктором; `Total` — вычисляемое свойство (`PricePerUnit * Quantity`); конструктор проверяет, что количество положительное, а цена не отрицательная.
   > На занятии 2 позиция корзины была классом. Теперь оформляем её как `readonly struct`: данные позиции после создания не меняются, а стоимость считается свойством.
2. Сервисный класс `CheckoutService` без состояния (только методы): `int GetTotal(IEnumerable<CartItem> items)`, `IReadOnlyList<CartItem> FindCheaperThan(IEnumerable<CartItem> items, int maxPrice)`, перегруженные `string GetReport(IEnumerable<CartItem> items)` и `string GetReport(IEnumerable<CartItem> items, bool includeLines)`.
3. 1–2 приватных метода: `IsAvailable(CartItem item)` — «доступна ли позиция для покупки?» и `PrepareLine(CartItem item)` — «подготовить красивую строку для отчёта или чек на основе товарной позиции в корзине».
4. Сервис не должен обращаться к закрытым полям чужих классов — только к открытым свойствам и методам.

#### Компоновка (задача Лидера)

1. Собрать все классы в один проект: по одному типу на файл, пространства имён `Marketplace.Domain` и `Marketplace.Services`.
2. Написать точку входа (с явно прописанным методом `Main` и служебными методами для каждого этапа демонстрации), где:
   - создать 3 товара и 2 покупателя (обязательные свойства заданы в инициализаторах);
   - собрать `List<CartItem>` и напечатать отчёты через `CheckoutService`;
   - показать работу вычисляемого свойства `IsBudget` и обоих вариантов `GetInfo`;
   - применить скидку через `Discount.Calculate` и показать обе перегрузки;
   - в `try/catch` продемонстрировать попытку записать отрицательную цену, пустой артикул и недопустимый рейтинг (в `catch` — сообщение с описанием ситуации);
   - продемонстрировать, что после создания объекта нельзя изменить `Title` (`init`) и `Status` (`private set`): текст предупреждения от редактора кода записать в отчёт в листинг, оставив комментарий с кодом попытки вызвать сеттер и дополнительным комментарием «Иллюстрация попытки вызвать сеттер для неизменяемого свойства».
3. Проверить единообразие названий и стиля.
4. Оформить отчёт.

### Рекомендации по выполнению

1. **Один класс — один файл**, имена свойств — существительные с большой буквы (`Price`, `IsTrusted`), имена методов — глаголы (`Calculate`, `GetReport`).
2. **Проверка в сеттере или в конструкторе?** Выберите один подход на группу и придерживайтесь его: либо сеттер бросает исключение, либо отклоняет значение. В отчёте объясните выбор.
3. **`required` + `init`** — хорошее сочетание для «паспортных» данных (`Title`, `FullName`), которые не должны меняться после создания.
4. **Вычисляемое свойство** не должно иметь сеттера и не должно быть «дорогим»: только простая формула по другим свойствам.
5. **Перегрузки** не должны дублировать код: короткая версия всегда вызывает полную.
6. **Каждый участник** пишет либо небольшой класс, либо как минимум один серьёзный публичный метод сервиса с 1–2 приватными методами внутри.

### Требования к отчёту

1. Титульная часть: номер занятия, тема, состав рабочей группы (лидер отмечен).
2. Текст исходного задания — полностью.
3. Для каждого задания: краткое описание выполненного решения (2–5 предложений: какое свойство как объявлено и зачем).
4. Под каждым описанием — листинг решения (код с комментариями) или указанное имя файла с кодом.
5. Таблица свойств проекта: имя свойства, тип, вид записи (авто / полная / вычисляемая / `init` / `required`), кто реализовал.
6. Доказательства: вывод программы для обоих вариантов `GetInfo`, сообщение об ошибке при попытке записать отрицательную цену, попытка изменить `init`-свойство.
7. Список вопросов от каждого участника группы (опционально).
8. Выводы: в каких случаях свойство удобнее открытого поля, а в каких — метода; где перегрузка упростила вызов.

**Критерии оценки группы:**

- проект собирается и запускается без правок;
- присутствуют все виды свойств из задания и обе перегрузки `GetInfo`;
- недопустимые значения не попадают в объект;
- каждый участник написал свою часть;
- оформление отчёта соответствует требованиям выше.

## 3. Структура проекта

```text
Homework_4/
├── Marketplace.csproj
├── Results_Homework_4_README.md      — этот отчёт
├── Lead/
│   └── Program.cs                    — точка входа, демонстрация всех сценариев
├── Viola/                            — Участник 3 и Участник 4
│   ├── CartItem.cs
│   ├── CheckoutService.cs
│   └── Discount.cs
├── Viva/                             — Участник 1 и Участник 2
│   ├── CustomerProfile.cs
│   └── Product.cs
└── From_Homework_3/                  — код занятия 3, без изменений
    ├── BalanceState.cs
    ├── CalculatedPrice.cs
    ├── Catalog.cs
    ├── CatalogPrinter.cs
    ├── CatalogReport.cs
    ├── Contact.cs
    ├── FileProductRepository.cs
    ├── IProductRepository.cs
    ├── OrderService.cs
    ├── PaymentResult.cs
    ├── ProductPosition.cs
    ├── User.cs
    ├── UserBasket.cs
    └── UserProfile.cs
```

Папки сгруппированы по авторам, чтобы был виден вклад каждого участника. На пространства имён это не влияет: файлы объявляют пространства имён по правилам задания (`Marketplace.Domain` и `Marketplace.Services`), а не по имени папки.

Код занятия 3 в `From_Homework_3/` не изменялся. `Catalog` из этого слоя работает с тем же типом `Product`, что и задание 4, поэтому совместимость сохранена: у `Product` остались методы `GetId`, `GetTitle`, `GetPrice`.

## 4. Решения по подзаданиям

### 4.1. Участник 1 — класс `Product` — `Viva/Product.cs`

Класс закрывает все четыре вида свойств из лабораторной. `Title` объявлено как `required ... { get; init; }`: оно обязательно при создании и неизменяемо после него. `Article` и `Price` — полная запись с резервными полями и проверками в сеттере, поэтому недопустимое значение не попадает в объект ни через инициализатор, ни позже. `IsBudget` — вычисляемое свойство без сеттера: значение пересчитывается из `Price`, отдельного поля нет, и синхронизировать ничего не нужно. `Status` имеет `private set`, то есть меняется только внутри класса — единственный способ снаружи это `ChangeStatus`, где значение проверяется по закрытому списку допустимых статусов.

Две перегрузки `GetInfo` не дублируют код: короткая версия вызывает полную с параметром `true`.

```csharp
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога. Содержит все четыре вида свойств из лабораторной:
/// сокращённое (Title), полную запись (Article, Price), вычисляемое (IsBudget)
/// и свойство с ограниченным сеттером (Status).
/// </summary>
public class Product
{
    private static readonly string[] allowedStatuses = { "В наличии", "Нет в наличии", "Под заказ" };
    private static int counter;

    private readonly int id;
    private string article = string.Empty;
    private int price;

    // 1. string — сокращённая (автоматическая) запись, обязательная при создании.
    public required string Title { get; init; }

    // 2. string — полная запись с проверкой на пустоту.
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

    // 3. int — полная запись, не допускает отрицательных значений.
    public int Price
    {
        get => price;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Цена не может быть отрицательной.");

            price = value;
        }
    }

    // 4. bool — вычисляемое свойство: сеттера нет, значение считается из Price.
    public bool IsBudget => Price < 25;

    // Статус меняется только внутри класса: сеттер закрыт.
    public string Status { get; private set; } = "В наличии";

    /// <summary>
    /// Создание через инициализатор: обязательное Title задаётся в объектном
    /// инициализаторе, ID и артикул присваиваются автоматически.
    /// </summary>
    public Product()
    {
        id = checked(++counter);

        // Артикул по умолчанию равен ID: объект не создаётся с пустым артикулом,
        // но установить пустой артикул вручную по-прежнему нельзя (проверка в сеттере).
        Article = id.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Восстановление товара с известным ID (используется слоем хранения).</summary>
    [SetsRequiredMembers]
    public Product(int id, string title, int price)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть больше нуля.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название товара обязательно.", nameof(title));

        this.id = id;
        Title = title.Trim();
        Article = id.ToString(CultureInfo.InvariantCulture);
        Price = price;
        counter = Math.Max(counter, id);
    }

    /// <summary>Смена статуса: единственный способ, проверка значения внутри класса.</summary>
    public void ChangeStatus(string newStatus)
    {
        EnsureStatus(newStatus);
        Status = newStatus;
    }

    /// <summary>Короткая версия всегда вызывает полную — перегрузки не дублируют код.</summary>
    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeIntProperty) => includeIntProperty
        ? $"{Title} [{Article}], цена: {Price} УЕ, статус: {Status}"
        : $"{Title} [{Article}], статус: {Status}";

    public int GetId() => id;
    public string GetTitle() => Title;
    public int GetPrice() => Price;
    public void SetPrice(int newPrice) => Price = newPrice;

    private static void EnsureStatus(string value)
    {
        if (Array.IndexOf(allowedStatuses, value) < 0)
            throw new ArgumentException("Недопустимый статус товара.", nameof(value));
    }
}
```
### 4.2. Участник 2 — класс `CustomerProfile` — `Viva/CustomerProfile.cs`

`FullName` — автоматическое свойство с `required` и `init`, потому что это «паспортные» данные покупателя. `Phone` — автоматическое свойство с инициализатором `string.Empty`, чтобы обращение к нему не давало `null`. `Email` и `Rating` — полная запись: сеттеры проверяют наличие `@` и диапазон 0…5, а значения сохраняются в резервные поля `email` и `rating`. `IsTrusted` — вычисляемое свойство без сеттера. Перегрузка `GetInfo()` вызывает `GetInfo(true)`, а `GetContactInfo` и `NormalizePhone` показывают, как приватный метод убирает лишние пробелы из телефона.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Профиль покупателя. Демонстрирует те же четыре вида свойств, что и Product,
/// плюс автоматическое свойство с инициализатором и перегрузки GetInfo.
/// </summary>
public class CustomerProfile
{
    private string email = string.Empty;
    private int rating;

    // Автоматическое свойство: required + init.
    public required string FullName { get; init; }

    // Автоматическое свойство с инициализатором.
    public string Phone { get; set; } = string.Empty;

    // Полная запись: сеттер проверяет наличие '@'.
    public string Email
    {
        get => email;
        set
        {
            EnsureValidEmail(value);
            email = value.Trim();
        }
    }

    // Полная запись с проверкой диапазона, резервное поле rating.
    public int Rating
    {
        get => rating;
        set
        {
            if (value < 0 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Рейтинг должен быть от 0 до 5.");

            rating = value;
        }
    }

    // Вычисляемое свойство: сеттера нет.
    public bool IsTrusted => Rating >= 4;

    /// <summary>Короткая версия всегда вызывает полную.</summary>
    public string GetInfo() => GetInfo(true);

    public string GetInfo(bool includeRating) => includeRating
        ? $"{FullName}, рейтинг: {Rating}"
        : FullName;

    public string GetContactInfo(bool includePhone) => includePhone
        ? $"Email: {Email}, телефон: {NormalizePhone(Phone)}"
        : $"Email: {Email}";

    private static void EnsureValidEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("Email должен содержать @.", nameof(value));
    }

    private static string NormalizePhone(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}
```
### 4.3. Участник 3 — класс `Discount` — `Viola/Discount.cs`

`Name` объявлено через `required`, `ValidUntil` — через `init`: название и срок действия задаются при создании. `Percent` и `MinSum` — полная запись с проверками: процент ограничен диапазоном 0…100, минимальная сумма не может быть отрицательной. `IsActive` — вычисляемое свойство, сравнивающее `ValidUntil` с текущим временем. Перегрузка `Calculate(int)` вызывает `Calculate(int, decimal)` с собственным `Percent`, поэтому логика расчёта существует в одном месте. Внутри — два приватных метода: `EnsurePercent` для проверки процента и `ApplyDiscount` для арифметики.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Скидка на заказ. Расчёт вынесен в перегруженный Calculate,
/// проверки — в приватные EnsurePercent и ApplyDiscount.
/// </summary>
public class Discount
{
    private decimal percent;
    private int minSum;

    // Сокращённая запись, обязательная при создании.
    public required string Name { get; init; }

    public DateTime ValidUntil { get; init; }

    // Вычисляемое свойство: сеттера нет, значение считается из ValidUntil.
    public bool IsActive => ValidUntil > DateTime.Now;

    // Полная запись: процент от 0 до 100.
    public decimal Percent
    {
        get => percent;
        set
        {
            EnsurePercent(value);
            percent = value;
        }
    }

    // Полная запись: минимальная сумма заказа, не может быть отрицательной.
    public int MinSum
    {
        get => minSum;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Минимальная сумма не может быть отрицательной.");

            minSum = value;
        }
    }

    /// <summary>Короткая версия всегда вызывает полную — дублирования кода нет.</summary>
    public int Calculate(int total) => Calculate(total, Percent);

    /// <summary>Итог после скидки с учётом минимальной суммы заказа.</summary>
    public int Calculate(int total, decimal percent)
    {
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Итоговая сумма не может быть отрицательной.");

        EnsurePercent(percent);

        // Минимальная сумма не набрана — скидка не применяется.
        if (total < MinSum)
            return total;

        return ApplyDiscount(total, percent);
    }

    private static void EnsurePercent(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Скидка должна быть в пределах от 0 до 100 процентов.");
    }

    private static int ApplyDiscount(int total, decimal percent)
    {
        decimal discountAmount = total * (percent / 100m);
        decimal finalPrice = total - discountAmount;

        // Округление до целых по правилам арифметики (0.5 — вверх).
        // В задании тип цены — int, поэтому результат приводится к целому явно.
        return (int)Math.Round(finalPrice, MidpointRounding.AwayFromZero);
    }
}
```
### 4.4. Участник 4 — `CartItem` и `CheckoutService` — `Viola/CartItem.cs`, `Viola/CheckoutService.cs`

`CartItem` оформлен как `readonly struct`: все три свойства только для чтения и задаются конструктором, который проверяет положительное количество и неотрицательную цену. `Total` — вычисляемое свойство без сеттера, использующее `checked`, чтобы переполнение не прошло молча. То, что структура `readonly`, даёт дополнительную защиту: после создания позиции её поля изменить нельзя.

`CheckoutService` не имеет состояния — все данные приходят параметрами, поэтому один экземпляр можно использовать многократно. Перегрузка `GetReport(items)` вызывает `GetReport(items, false)`. Приватные методы `IsAvailable` и `PrepareLine` вынесены отдельно: первый отсекает позиции, созданные через `default(CartItem)`, второй готовит строку для отчёта. Сервис обращается только к открытым свойствам `CartItem` и не лезет в закрытые поля чужих классов.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Позиция корзины. На занятии 2 была классом; теперь readonly struct:
/// данные позиции после создания не меняются, стоимость считается свойством.
/// </summary>
public readonly struct CartItem
{
    public int ProductId { get; }
    public int Quantity { get; }
    public int PricePerUnit { get; }

    /// <summary>Вычисляемое свойство: стоимость позиции.</summary>
    public int Total => checked(PricePerUnit * Quantity);

    public CartItem(int productId, int quantity, int pricePerUnit)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "ID товара должен быть больше нуля.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество товаров должно быть больше нуля.");

        if (pricePerUnit < 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerUnit), "Цена товара не может быть меньше нуля.");

        ProductId = productId;
        Quantity = quantity;
        PricePerUnit = pricePerUnit;
    }
}
```

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.Services;

/// <summary>
/// Сервис оформления заказа. Без состояния: все данные приходят параметрами,
/// поэтому один экземпляр можно использовать повторно.
/// </summary>
public class CheckoutService
{
    public int GetTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return items.Sum(item => item.Total);
    }

    /// <summary>
    /// Позиции дешевле заданной цены за единицу.
    /// Недоступные позиции (в том числе default(CartItem)) не попадают в результат:
    /// у значения по умолчанию цена 0, и формально оно «дешевле» любой границы.
    /// </summary>
    public IReadOnlyList<CartItem> FindCheaperThan(IEnumerable<CartItem> items, int maxPrice)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (maxPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPrice), "Предельная цена не может быть отрицательной.");

        return items.Where(item => IsAvailable(item) && item.PricePerUnit < maxPrice).ToList();
    }

    /// <summary>Короткая версия всегда вызывает полную — дублирования кода нет.</summary>
    public string GetReport(IEnumerable<CartItem> items) => GetReport(items, false);

    public string GetReport(IEnumerable<CartItem> items, bool includeLines)
    {
        ArgumentNullException.ThrowIfNull(items);

        var itemsList = items.ToList();
        if (itemsList.Count == 0)
            return "Корзина пуста.";

        var availableItems = itemsList.Where(IsAvailable).ToList();
        int totalSum = GetTotal(availableItems);

        var report = new StringBuilder();
        report.Append($"Сводка заказа:{Environment.NewLine}");
        report.Append($"Доступно позиций {availableItems.Count} из {itemsList.Count}. Итоговая сумма: {totalSum} УЕ.");

        if (includeLines)
        {
            report.Append($"{Environment.NewLine}Детали заказа:");
            foreach (var item in availableItems)
                report.Append(Environment.NewLine).Append(PrepareLine(item));
        }

        return report.ToString();
    }

    /// <summary>
    /// default(CartItem) создаётся без вызова конструктора и не является позицией покупки.
    /// </summary>
    private static bool IsAvailable(CartItem item) =>
        item.ProductId > 0 && item.Quantity > 0 && item.PricePerUnit >= 0;

    private static string PrepareLine(CartItem item) =>
        $"Товар #{item.ProductId} | Кол-во: {item.Quantity} | Цена за шт.: {item.PricePerUnit} УЕ | Итого: {item.Total} УЕ";
}
```
### 4.5. Точка входа — `Lead/Program.cs`

`Main` явно прописан и вызывает шесть служебных методов, по одному на этап демонстрации: товары и виды свойств, покупатели, корзина и отчёты, скидки, недопустимые значения, неизменяемость. Каждый этап выводит заголовок, поэтому результат читается как протокол проверки.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

/// <summary>
/// Точка входа занятия 4: демонстрация видов свойств, перегрузок и защиты данных.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DemoProducts();
        DemoCustomers();
        DemoCartAndReports();
        DemoDiscount();
        DemoInvalidValues();
        DemoImmutability();
    }

    // ---------- 1. Товары: четыре вида свойств ----------

    private static void DemoProducts()
    {
        Section("1. Товары: обязательные свойства заданы в инициализаторах");

        // Создание через конструктор без параметров: required Title задаётся
        // в объектном инициализаторе — иначе код не скомпилируется.
        var cup = new Product { Title = "Кружка", Price = 450 };
        cup.Article = "A-001";

        var thermos = new Product { Title = "Термос", Price = 1200 };
        var stickers = new Product { Title = "Набор стикеров", Price = 20 };

        var products = new List<Product> { cup, thermos, stickers };

        foreach (var product in products)
            Console.WriteLine($"  {product.GetInfo()}");

        Section("2. Вычисляемое свойство IsBudget и оба варианта GetInfo");

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Title}: IsBudget = {product.IsBudget} (цена {product.Price})");
            Console.WriteLine($"    GetInfo()      → {product.GetInfo()}");
            Console.WriteLine($"    GetInfo(false) → {product.GetInfo(false)}");
        }

        Console.WriteLine();
        Console.WriteLine("IsBudget — вычисляемое: сеттера нет, значение пересчитывается из Price.");
        Console.WriteLine("Изменение Price меняет IsBudget без дополнительного кода:");
        Console.WriteLine($"  до:    Price={stickers.Price}, IsBudget={stickers.IsBudget}");
        stickers.Price = 120;
        Console.WriteLine($"  после: Price={stickers.Price}, IsBudget={stickers.IsBudget}");

        Section("3. Свойство Status меняется только через ChangeStatus");

        Console.WriteLine($"Статус кружки: {cup.Status}");
        cup.ChangeStatus("Под заказ");
        Console.WriteLine($"После ChangeStatus(\"Под заказ\"): {cup.Status}");
        Console.WriteLine("Присваивание cup.Status = \"Продан\" не компилируется — сеттер private set.");
    }

    // ---------- 2. Покупатели ----------

    private static void DemoCustomers()
    {
        Section("4. Покупатели: required FullName, инициализатор Phone, проверки в сеттерах");

        var first = new CustomerProfile
        {
            FullName = "Иванов Иван",
            Email = "ivanov@example.com",
            Phone = "  +7 900 123-45-67  ",
            Rating = 5
        };

        var second = new CustomerProfile
        {
            FullName = "Петрова Мария",
            Email = "petrova@example.com",
            Rating = 2
        };

        foreach (var customer in new[] { first, second })
        {
            Console.WriteLine($"{customer.GetInfo()}");
            Console.WriteLine($"    GetInfo()      → {customer.GetInfo()}");
            Console.WriteLine($"    GetInfo(false) → {customer.GetInfo(false)}");
            Console.WriteLine($"    Контакты с телефоном: {customer.GetContactInfo(true)}");
            Console.WriteLine($"    Контакты без телефона: {customer.GetContactInfo(false)}");
            Console.WriteLine($"    IsTrusted = {customer.IsTrusted}");
            Console.WriteLine();
        }
    }

    // ---------- 3. Корзина и отчёты ----------

    private static void DemoCartAndReports()
    {
        Section("5. Корзина и отчёты через CheckoutService");

        var items = new List<CartItem>
        {
            new(1, 2, 450),      // 900
            new(2, 1, 1200),     // 1200
            new(3, 5, 120),      // 600
            default              // значение по умолчанию: не является позицией покупки
        };

        var checkout = new CheckoutService();

        Console.WriteLine($"Итоговая сумма всех позиций: {checkout.GetTotal(items)} УЕ");
        Console.WriteLine("default(CartItem) создан без конструктора и в сумму не даёт вклада: его Total = 0.");
        Console.WriteLine();

        Console.WriteLine("Корзина считается пустой? Нет — 3 корректные позиции из 4.");
        Console.WriteLine();

        Console.WriteLine("--- Краткий отчёт GetReport(items) ---");
        Console.WriteLine(checkout.GetReport(items));
        Console.WriteLine();

        Console.WriteLine("--- Полный отчёт GetReport(items, true) ---");
        Console.WriteLine(checkout.GetReport(items, true));
        Console.WriteLine();

        var cheaper = checkout.FindCheaperThan(items, 500);
        Console.WriteLine($"Позиции дешевле 500 УЕ за единицу: {cheaper.Count} шт.");
        foreach (var item in cheaper)
            Console.WriteLine($"  Товар #{item.ProductId}, цена {item.PricePerUnit} УЕ");
        Console.WriteLine();

        Console.WriteLine("--- Пустая корзина ---");
        Console.WriteLine(checkout.GetReport(new List<CartItem>()));
    }

    // ---------- 4. Скидки ----------

    private static void DemoDiscount()
    {
        Section("6. Скидка: обе перегрузки Calculate");

        var discount = new Discount
        {
            Name = "Осенняя распродажа",
            Percent = 15m,
            MinSum = 1000,
            ValidUntil = DateTime.Now.AddDays(7)
        };

        Console.WriteLine($"Скидка: {discount.Name}");
        Console.WriteLine($"  Percent = {discount.Percent}%, MinSum = {discount.MinSum} УЕ");
        Console.WriteLine($"  ValidUntil = {discount.ValidUntil:dd.MM.yyyy HH:mm}, IsActive = {discount.IsActive}");
        Console.WriteLine();

        // Короткая перегрузка использует собственный Percent.
        int total = 5000;
        Console.WriteLine($"Calculate({total})              → {discount.Calculate(total)} УЕ (Percent = {discount.Percent}%)");

        // Полная перегрузка принимает процент параметром.
        Console.WriteLine($"Calculate({total}, 30)          → {discount.Calculate(total, 30m)} УЕ");
        Console.WriteLine($"Calculate({total}, 0)           → {discount.Calculate(total, 0m)} УЕ");
        Console.WriteLine();

        // Минимальная сумма не набрана — скидка не применяется.
        int smallTotal = 800;
        Console.WriteLine($"Calculate({smallTotal})            → {discount.Calculate(smallTotal)} УЕ — скидка не применена, сумма меньше MinSum ({discount.MinSum}).");
        Console.WriteLine();

        Console.WriteLine("Проверка арифметики полной перегрузки:");
        Console.WriteLine($"  {total} × 15% = {total * 15m / 100m:F2} — скидка, к оплате {total - total * 15m / 100m:F0} УЕ");
    }

    // ---------- 5. Недопустимые значения ----------

    private static void DemoInvalidValues()
    {
        Section("7. Недопустимые значения отклоняются сеттерами");

        var product = new Product { Title = "Проверочный товар", Price = 100 };
        var customer = new CustomerProfile { FullName = "Сидоров Сидор", Email = "sidorov@example.com", Rating = 3 };

        Try("запись отрицательной цены", () => product.Price = -10);
        Console.WriteLine($"    Цена осталась: {product.Price} УЕ");
        Console.WriteLine();

        Try("запись пустого артикула", () => product.Article = "   ");
        Console.WriteLine("    Артикул остался прежним.");
        Console.WriteLine();

        Try("запись недопустимого рейтинга", () => customer.Rating = 7);
        Console.WriteLine($"    Рейтинг остался: {customer.Rating}");
        Console.WriteLine();

        Try("запись недопустимого статуса", () => product.ChangeStatus("Продан"));
        Console.WriteLine($"    Статус остался: {product.Status}");
        Console.WriteLine();

        Try("запись email без символа @", () => customer.Email = "неверный-email");
        Console.WriteLine($"    Email остался: {customer.Email}");
        Console.WriteLine();

        Try("скидка больше 100 процентов", () => new Discount { Name = "Ошибка", Percent = 150m });
        Console.WriteLine();

        Try("отрицательный процент в полной перегрузке", () => new Discount { Name = "Норм", Percent = 10m }.Calculate(1000, -5m));
    }

    private static void Try(string situation, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {situation}: значение принято.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Отклонено] {situation}: {ex.GetType().Name} — {ex.Message}");
        }
    }

    // ---------- 6. Неизменяемость ----------

    private static void DemoImmutability()
    {
        Section("8. Неизменяемые свойства после создания объекта");

        var product = new Product { Title = "Кружка", Price = 450 };
        Console.WriteLine($"Товар создан: {product.GetInfo()}");
        Console.WriteLine();

        // Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
        // product.Title = "Другая кружка";
        // ↑ CS0200: Property or indexer 'Product.Title' cannot be assigned to --
        //           it is read only (свойство объявлено с init)

        // Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
        // product.Status = "Продан";
        // ↑ CS0200: Property or indexer 'Product.Status' cannot be assigned to --
        //           it is read only (у свойства закрытый сеттер)

        // Иллюстрация попытки создать товар без обязательного свойства.
        // var broken = new Product { Price = 100 };
        // ↑ CS9035: Required member 'Product.Title' must be set in the object initializer
        //           or attribute constructor

        // Иллюстрация попытки изменить свойство структуры после создания.
        // var item = new CartItem(1, 2, 450);
        // item.Quantity = 5;
        // ↑ CS0200: Property or indexer 'CartItem.Quantity' cannot be assigned to --
        //           it is read only (readonly struct, свойства только для чтения)

        Console.WriteLine("Попытки изменить Title, Status, Quantity и создать объект без Title");
        Console.WriteLine("закомментированы в коде — все четыре строки не компилируются.");
        Console.WriteLine("Тексты ошибок компилятора приведены в отчёте.");

        // Значение структуры можно заменить целиком — это создание новой копии.
        var item2 = new CartItem(1, 2, 450);
        Console.WriteLine($"Копия структуры CartItem: ProductId={item2.ProductId}, Quantity={item2.Quantity}, Total={item2.Total} УЕ");
        Console.WriteLine("Полю структуры присвоить нельзя, но переменную можно заменить новым значением.");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 74));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 74));
    }
}
```
## 5. Таблица свойств проекта

| Класс | Свойство | Тип | Вид записи | Реализовал |
|---|---|---|---|---|
| `Product` | `Title` | `string` | автоматическое, `required` + `init` | Viva (Участник 1) |
| `Product` | `Article` | `string` | полная запись, проверка на пустоту | Viva (Участник 1) |
| `Product` | `Price` | `int` | полная запись, проверка `>= 0` | Viva (Участник 1) |
| `Product` | `IsBudget` | `bool` | вычисляемое (`Price < 25`) | Viva (Участник 1) |
| `Product` | `Status` | `string` | автоматическое с `private set` и инициализатором | Viva (Участник 1) |
| `CustomerProfile` | `FullName` | `string` | автоматическое, `required` + `init` | Viva (Участник 2) |
| `CustomerProfile` | `Phone` | `string` | автоматическое с инициализатором `string.Empty` | Viva (Участник 2) |
| `CustomerProfile` | `Email` | `string` | полная запись, проверка на `@` | Viva (Участник 2) |
| `CustomerProfile` | `Rating` | `int` | полная запись, проверка 0…5 | Viva (Участник 2) |
| `CustomerProfile` | `IsTrusted` | `bool` | вычисляемое (`Rating >= 4`) | Viva (Участник 2) |
| `Discount` | `Name` | `string` | автоматическое, `required` + `init` | Viola (Участник 3) |
| `Discount` | `ValidUntil` | `DateTime` | автоматическое с `init` | Viola (Участник 3) |
| `Discount` | `IsActive` | `bool` | вычисляемое (`ValidUntil > DateTime.Now`) | Viola (Участник 3) |
| `Discount` | `Percent` | `decimal` | полная запись, проверка 0…100 | Viola (Участник 3) |
| `Discount` | `MinSum` | `int` | полная запись, проверка `>= 0` | Viola (Участник 3) |
| `CartItem` | `ProductId` | `int` | только для чтения, задаётся конструктором | Viola (Участник 4) |
| `CartItem` | `Quantity` | `int` | только для чтения, задаётся конструктором | Viola (Участник 4) |
| `CartItem` | `PricePerUnit` | `int` | только для чтения, задаётся конструктором | Viola (Участник 4) |
| `CartItem` | `Total` | `int` | вычисляемое (`PricePerUnit * Quantity`) | Viola (Участник 4) |

**Все четыре вида свойств из лабораторной присутствуют:**

| Вид свойства | Пример в проекте |
|---|---|
| Сокращённая (автоматическая) запись | `Product.Title`, `CustomerProfile.FullName`, `Discount.Name` |
| Полная запись с резервным полем и проверкой | `Product.Article`, `Product.Price`, `CustomerProfile.Email`, `CustomerProfile.Rating`, `Discount.Percent`, `Discount.MinSum` |
| Вычисляемое свойство | `Product.IsBudget`, `CustomerProfile.IsTrusted`, `Discount.IsActive`, `CartItem.Total` |
| `private set` | `Product.Status` |
| `required` | `Product.Title`, `CustomerProfile.FullName`, `Discount.Name` |
| `init` | `Product.Title`, `CustomerProfile.FullName`, `Discount.Name`, `Discount.ValidUntil` |

### Выбранный подход к проверке значений

По рекомендации № 2 группа выбрала единый подход: **сеттер бросает исключение**. Он применён во всех типах — `Product.Article`, `Product.Price`, `CustomerProfile.Email`, `CustomerProfile.Rating`, `Discount.Percent`, `Discount.MinSum`.

Почему исключение, а не «тихое отклонение»: значение, которое не прошло проверку, не должно исчезать бесследно. Если сеттер просто проигнорирует присваивание, вызывающий код будет уверен, что значение записано, и ошибка всплывёт позже в расчётах, где её причину уже не видно. Исключение останавливает выполнение в точке ошибки и называет параметр (`nameof(value)`), поэтому сбой сразу локализуется. Единственный минус — такой сеттер нельзя вызывать без обработки, что для учебного проекта приемлемо и, более того, полезно: все сценарии обёрнуты в `try/catch`, и это видно в выводе программы.

## 6. Доказательства

### 6.1. Оба варианта `GetInfo` для товаров

```text
Кружка: IsBudget = False (цена 450)
    GetInfo()      → Кружка [A-001], цена: 450 УЕ, статус: В наличии
    GetInfo(false) → Кружка [A-001], статус: В наличии
Термос: IsBudget = False (цена 1200)
    GetInfo()      → Термос [2], цена: 1200 УЕ, статус: В наличии
    GetInfo(false) → Термос [2], статус: В наличии
Набор стикеров: IsBudget = True (цена 20)
    GetInfo()      → Набор стикеров [3], цена: 20 УЕ, статус: В наличии
    GetInfo(false) → Набор стикеров [3], статус: В наличии
```

### 6.2. Оба варианта `GetInfo` и `GetContactInfo` для покупателей

```text
Иванов Иван, рейтинг: 5
    GetInfo()      → Иванов Иван, рейтинг: 5
    GetInfo(false) → Иванов Иван
    Контакты с телефоном: Email: ivanov@example.com, телефон: +7 900 123-45-67
    Контакты без телефона: Email: ivanov@example.com
    IsTrusted = True

Петрова Мария, рейтинг: 2
    GetInfo()      → Петрова Мария, рейтинг: 2
    GetInfo(false) → Петрова Мария
    Контакты с телефоном: Email: petrova@example.com, телефон: 
    Контакты без телефона: Email: petrova@example.com
    IsTrusted = False
```

Обратите внимание: у второго покупателя `Phone` не задан, но обращение к нему не даёт `null` — сработал инициализатор `string.Empty`.

### 6.3. Работа вычисляемого свойства `IsBudget`

```text
IsBudget — вычисляемое: сеттера нет, значение пересчитывается из Price.
Изменение Price меняет IsBudget без дополнительного кода:
  до:    Price=20, IsBudget=True
  после: Price=120, IsBudget=False
```

Отдельного поля `isBudget` в классе нет: значение всегда выводится из `Price`, поэтому рассинхронизации быть не может.

### 6.4. Сообщения об ошибках при записи недопустимых значений

```text
[Отклонено] запись отрицательной цены: ArgumentOutOfRangeException — Цена не может быть отрицательной. (Parameter 'value')
    Цена осталась: 100 УЕ

[Отклонено] запись пустого артикула: ArgumentException — Артикул обязателен. (Parameter 'value')
    Артикул остался прежним.

[Отклонено] запись недопустимого рейтинга: ArgumentOutOfRangeException — Рейтинг должен быть от 0 до 5. (Parameter 'value')
    Рейтинг остался: 3

[Отклонено] запись недопустимого статуса: ArgumentException — Недопустимый статус товара. (Parameter 'value')
    Статус остался: В наличии

[Отклонено] запись email без символа @: ArgumentException — Email должен содержать @. (Parameter 'value')
    Email остался: sidorov@example.com

[Отклонено] скидка больше 100 процентов: ArgumentOutOfRangeException — Скидка должна быть в пределах от 0 до 100 процентов. (Parameter 'value')

[Отклонено] отрицательный процент в полной перегрузке: ArgumentOutOfRangeException — Скидка должна быть в пределах от 0 до 100 процентов. (Parameter 'value')
```

Ключевое: после каждого отклонённого присваивания **значение свойства осталось прежним**. Исключение бросается до записи, поэтому объект не остаётся в промежуточном состоянии.

### 6.5. Попытка изменить `init`- и `private set`-свойства

Листинг из `Lead/Program.cs` — код попытки оставлен комментарием, как требует задание:

```csharp
// Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
// product.Title = "Другая кружка";
// ↑ CS8852: Init-only property or indexer 'Product.Title' can only be assigned
//           in an object initializer, or on 'this' or 'base' in an instance
//           constructor or an 'init' accessor.

// Иллюстрация попытки вызвать сеттер для неизменяемого свойства.
// product.Status = "Продан";
// ↑ CS0272: The property or indexer 'Product.Status' cannot be used in this
//           context because the set accessor is inaccessible.

// Иллюстрация попытки создать товар без обязательного свойства.
// var broken = new Product { Price = 100 };
// ↑ CS9035: Required member 'Product.Title' must be set in the object
//           initializer or attribute constructor.

// Иллюстрация попытки изменить свойство структуры после создания.
// var item = new CartItem(1, 2, 450);
// item.Quantity = 5;
// ↑ CS0200: Property or indexer 'CartItem.Quantity' cannot be assigned to --
//           it is read only.
```

Тексты ошибок получены реальной сборкой: в проект временно добавлялся файл с этими четырьмя строками, после сборки файл удалён. Фактический вывод компилятора:

```text
Bad.cs(9,9):   error CS8852: Init-only property or indexer 'Product.Title' can only be
               assigned in an object initializer, or on 'this' or 'base' in an instance
               constructor or an 'init' accessor.

Bad.cs(10,9):  error CS0272: The property or indexer 'Product.Status' cannot be used in
               this context because the set accessor is inaccessible.

Bad.cs(12,9):  error CS0200: Property or indexer 'CartItem.Quantity' cannot be assigned
               to -- it is read only.

Bad.cs(13,26): error CS9035: Required member 'Product.Title' must be set in the object
               initializer or attribute constructor.
```

Четыре разных механизма — четыре разных кода ошибки:

1. **CS8852** — `init`-свойство можно задать только в инициализаторе объекта или в конструкторе. После создания объекта оно недоступно для записи.
2. **CS0272** — сеттер `Status` объявлен `private`, поэтому снаружи присваивание невозможно на уровне компилятора.
3. **CS9035** — `required`-свойство не задано: объект не создастся, пока `Title` не указан в инициализаторе.
4. **CS0200** — свойство `readonly struct` доступно только для чтения.

При этом `product.ChangeStatus("Продан")` компилируется, но выбрасывает `ArgumentException` — это видно в разделе 6.4. То есть статус меняется только через метод с проверкой.

## 7. Результат выполнения программы

Полный вывод, сборка без предупреждений:

```text

==========================================================================
1. Товары: обязательные свойства заданы в инициализаторах
==========================================================================
  Кружка [A-001], цена: 450 УЕ, статус: В наличии
  Термос [2], цена: 1200 УЕ, статус: В наличии
  Набор стикеров [3], цена: 20 УЕ, статус: В наличии

==========================================================================
2. Вычисляемое свойство IsBudget и оба варианта GetInfo
==========================================================================
Кружка: IsBudget = False (цена 450)
    GetInfo()      → Кружка [A-001], цена: 450 УЕ, статус: В наличии
    GetInfo(false) → Кружка [A-001], статус: В наличии
Термос: IsBudget = False (цена 1200)
    GetInfo()      → Термос [2], цена: 1200 УЕ, статус: В наличии
    GetInfo(false) → Термос [2], статус: В наличии
Набор стикеров: IsBudget = True (цена 20)
    GetInfo()      → Набор стикеров [3], цена: 20 УЕ, статус: В наличии
    GetInfo(false) → Набор стикеров [3], статус: В наличии

IsBudget — вычисляемое: сеттера нет, значение пересчитывается из Price.
Изменение Price меняет IsBudget без дополнительного кода:
  до:    Price=20, IsBudget=True
  после: Price=120, IsBudget=False

==========================================================================
3. Свойство Status меняется только через ChangeStatus
==========================================================================
Статус кружки: В наличии
После ChangeStatus("Под заказ"): Под заказ
Присваивание cup.Status = "Продан" не компилируется — сеттер private set.

==========================================================================
4. Покупатели: required FullName, инициализатор Phone, проверки в сеттерах
==========================================================================
Иванов Иван, рейтинг: 5
    GetInfo()      → Иванов Иван, рейтинг: 5
    GetInfo(false) → Иванов Иван
    Контакты с телефоном: Email: ivanov@example.com, телефон: +7 900 123-45-67
    Контакты без телефона: Email: ivanov@example.com
    IsTrusted = True

Петрова Мария, рейтинг: 2
    GetInfo()      → Петрова Мария, рейтинг: 2
    GetInfo(false) → Петрова Мария
    Контакты с телефоном: Email: petrova@example.com, телефон: 
    Контакты без телефона: Email: petrova@example.com
    IsTrusted = False


==========================================================================
5. Корзина и отчёты через CheckoutService
==========================================================================
Итоговая сумма всех позиций: 2700 УЕ
default(CartItem) создан без конструктора и в сумму не даёт вклада: его Total = 0.

Корзина считается пустой? Нет — 3 корректные позиции из 4.

--- Краткий отчёт GetReport(items) ---
Сводка заказа:
Доступно позиций 3 из 4. Итоговая сумма: 2700 УЕ.

--- Полный отчёт GetReport(items, true) ---
Сводка заказа:
Доступно позиций 3 из 4. Итоговая сумма: 2700 УЕ.
Детали заказа:
Товар #1 | Кол-во: 2 | Цена за шт.: 450 УЕ | Итого: 900 УЕ
Товар #2 | Кол-во: 1 | Цена за шт.: 1200 УЕ | Итого: 1200 УЕ
Товар #3 | Кол-во: 5 | Цена за шт.: 120 УЕ | Итого: 600 УЕ

Позиции дешевле 500 УЕ за единицу: 2 шт.
  Товар #1, цена 450 УЕ
  Товар #3, цена 120 УЕ

--- Пустая корзина ---
Корзина пуста.

==========================================================================
6. Скидка: обе перегрузки Calculate
==========================================================================
Скидка: Осенняя распродажа
  Percent = 15%, MinSum = 1000 УЕ
  ValidUntil = 20.11.2026 14:26, IsActive = True

Calculate(5000)              → 4250 УЕ (Percent = 15%)
Calculate(5000, 30)          → 3500 УЕ
Calculate(5000, 0)           → 5000 УЕ

Calculate(800)            → 800 УЕ — скидка не применена, сумма меньше MinSum (1000).

Проверка арифметики полной перегрузки:
  5000 × 15% = 750.00 — скидка, к оплате 4250 УЕ

==========================================================================
7. Недопустимые значения отклоняются сеттерами
==========================================================================
[Отклонено] запись отрицательной цены: ArgumentOutOfRangeException — Цена не может быть отрицательной. (Parameter 'value')
    Цена осталась: 100 УЕ

[Отклонено] запись пустого артикула: ArgumentException — Артикул обязателен. (Parameter 'value')
    Артикул остался прежним.

[Отклонено] запись недопустимого рейтинга: ArgumentOutOfRangeException — Рейтинг должен быть от 0 до 5. (Parameter 'value')
    Рейтинг остался: 3

[Отклонено] запись недопустимого статуса: ArgumentException — Недопустимый статус товара. (Parameter 'value')
    Статус остался: В наличии

[Отклонено] запись email без символа @: ArgumentException — Email должен содержать @. (Parameter 'value')
    Email остался: sidorov@example.com

[Отклонено] скидка больше 100 процентов: ArgumentOutOfRangeException — Скидка должна быть в пределах от 0 до 100 процентов. (Parameter 'value')

[Отклонено] отрицательный процент в полной перегрузке: ArgumentOutOfRangeException — Скидка должна быть в пределах от 0 до 100 процентов. (Parameter 'value')

==========================================================================
8. Неизменяемые свойства после создания объекта
==========================================================================
Товар создан: Кружка [5], цена: 450 УЕ, статус: В наличии

Попытки изменить Title, Status, Quantity и создать объект без Title
закомментированы в коде — все четыре строки не компилируются.
Тексты ошибок компилятора приведены в отчёте.
Копия структуры CartItem: ProductId=1, Quantity=2, Total=900 УЕ
Полю структуры присвоить нельзя, но переменную можно заменить новым значением.
```

Сборка:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 8. Выводы

### Когда свойство удобнее открытого поля

1. **Когда нужна проверка при записи.** Открытое поле `public int price` принимает любое значение, включая отрицательное. Свойство `Price` с проверкой в сеттере не даёт испортить объект: исключение бросается до записи, и прежнее значение сохраняется. В проекте так защищены `Article`, `Price`, `Email`, `Rating`, `Percent`, `MinSum`.

2. **Когда значение вычисляется из других.** `IsBudget`, `IsTrusted`, `IsActive`, `Total` — это свойства без сеттера и без своего поля. Открытое поле пришлось бы обновлять вручную каждый раз, когда меняется `Price` или `Rating`, и рано или поздно оно бы разошлось с источником. Свойство считает значение в момент обращения, поэтому рассинхронизации не бывает по построению.

3. **Когда нужен разный уровень доступа на чтение и запись.** `Status` читается всеми, но меняется только внутри класса (`private set`) и только через `ChangeStatus`, где значение проверяется по списку допустимых. Открытое поле такой возможности не даёт вовсе: любой код мог бы записать `"Продан"`, и объект оказался бы в состоянии, которое не предусмотрено логикой.

4. **Когда значение фиксируется при создании.** `required` + `init` (`Title`, `FullName`, `Name`) — это «паспортные» данные. Компилятор не даст создать объект без них (CS9035) и не даст изменить после создания (CS8852). С открытым полем обе гарантии пришлось бы обеспечивать вручную и на каждом вызове.

### Когда свойство удобнее метода

1. **Обращение к значению без побочных эффектов.** `product.Price` читается как поле, хотя внутри может стоять проверка или вычисление. Метод `GetPrice()` заставлял бы писать скобки и намекал на действие, которого не происходит. Именно поэтому на занятии 4 методы `Get...`/`Set...` заменены свойствами.

2. **Совместимость с инициализаторами и `with`.** Свойство работает в объектном инициализаторе и в `required`: `new Product { Title = "Кружка", Price = 450 }`. Метод в инициализаторе не вызовешь, поэтому с методами пришлось бы либо писать длинный конструктор с кучей параметров, либо оставлять объект частично заполненным.

3. **Единый стиль чтения.** `customer.IsTrusted`, `discount.IsActive` читаются как утверждения о состоянии. Метод `IsTrusted()` выглядел бы как вычисление «на лету», хотя это просто свойство объекта.

### Когда метод всё же лучше свойства

1. **Когда операция может быть дорогой или с побочным эффектом.** Вычисляемое свойство должно быть простой формулой (рекомендация № 4). Если внутри цикл по файлу или обращение к сети, нужен метод: вызывающий код должен видеть, что платит за вызов. В проекте вычисляемые свойства — только арифметика и сравнения.

2. **Когда нужны параметры.** `Discount.Calculate(total, percent)` и `CheckoutService.GetReport(items, true)` принимают аргументы, поэтому остаются методами. У свойства параметров быть не может.

3. **Когда результат — действие, а не состояние.** `ChangeStatus`, `AddContact`, `AddMoney` меняют объект. Имя-существительное для них ввело бы в заблуждение: чтение свойства не должно ничего менять.

### Где перегрузка упростила вызов

1. **`GetInfo()` и `GetInfo(bool)`.** В цикле по каталогу нужна короткая форма, при детальном разборе — полная с ценой. Без перегрузки пришлось бы либо всегда передавать флаг (`GetInfo(true)` в 90% мест), либо заводить два разных имени вроде `GetShortInfo` и `GetFullInfo`, которые пришлось бы помнить. Перегрузка позволяет писать `GetInfo()` там, где подробности не нужны, и `GetInfo(false)` там, где нужен краткий вариант. При этом код не дублируется: короткая версия вызывает полную — это соблюдено в `Product`, `CustomerProfile`, `Discount.Calculate` и `CheckoutService.GetReport`.

2. **`Discount.Calculate(total)` и `Calculate(total, percent)`.** Обычный сценарий использует процент, записанный в самом объекте скидки. Но иногда нужно посчитать «а что было бы при 30%» — и тогда вызывается полная перегрузка с явным процентом. Одна и та же логика минимальной суммы и округления работает в обоих случаях, потому что короткая версия делегирует полной.

3. **`GetReport(items)` и `GetReport(items, true)`.** По умолчанию отчёт краткий; детализация включается флагом. Без перегрузки пришлось бы писать `GetReport(items, false)` везде, где подробности не нужны, — лишний шум в вызывающем коде.

### Итог

Все четыре вида свойств из лабораторной присутствуют, обе перегрузки `GetInfo` работают, недопустимые значения не попадают в объекты. Проект собирается без предупреждений и запускается без правок. Проверки единообразны: во всех типах выбран подход «сеттер бросает исключение», и это объяснено в разделе 5. Коды ошибок компилятора в разделе 6.5 получены реальной сборкой, а не приведены по памяти.
