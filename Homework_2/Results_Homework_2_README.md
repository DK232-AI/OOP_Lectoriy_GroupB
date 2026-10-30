# ООП 2026 — Домашнее задание 2

## 1. Титульная часть

| Параметр | Значение |
|---|---|
| Номер занятия | 2 |
| Тема | Структуры в C#: значимые типы, копирование значений, стек и куча, `with` |
| Вариант проекта | Marketplace — Корзина Покупателя с бонусным балансом |
| Состав рабочей группы | 3 человека |

**Состав рабочей группы 251-337 Б:**

| Участник | Роль | Папка | Зона ответственности |
|---|---|---|---|
| Дмитриев Кирилл Денисович | **Лидер группы** | `Lead/` | Компоновка проекта, сервис оплаты, точка входа, отчёт |
| Астафьева Виола Александровна | Участник | `Viola/` | Класс `UserProfile`, структура `BalanceState` |
| Мирошниченко Владимир Сергеевич | Участник | `Viva/` | Классы `Product`, `ProductPosition`, `UserBasket`, `User`, структура `CalculatedPrice` |

## 2. Текст исходного задания

### Задание 3 (групповое)

Продолжаем развивать наш проект с Пользователем.

Теперь нашей целью будет разработка Корзины Покупателя, которую можно наполнить Товарами, имеющими Цену в условных денежных единицах, часть которой можно заплатить бонусными баллами, а для расчёта финальной цены на товары в корзине мы должны учесть Персональную Скидку Пользователя и его Баланс Бонусных Баллов.

Для осуществления задуманного нам потребуется реализовать следующие классы и структуры:

1. класс Товара (ID, Название, цена за единицу);
2. класс Товарной Позиции в Корзине (ID, ID Товара, количество единиц товара в позиции);
3. класс Корзины Пользователя (ID, ID Пользователя, список Товарных Позиций);
4. структуру Расчётной Цены корзины (считаем, что все товары могут быть проданы со скидкой) (Исходная цена, Скидка в виде множителя от 0.01 до 1.00, где 0.01 — скидка 99%, а 1.00 — скидка 0%, Размер Скидки в Деньгах);
5. структуру Состояния Баланса Покупателя (содержащую количество бонусов и количество денежных единиц, а также долю итоговой цены, которую можно оплатить бонусами в виде множителя от 0.0 до 0.5, где 0.0 — это 0%, и 0.5 — это 50% итоговой цены можно оплатить бонусами).

Кроме того, нужно будет дополнить Профиль Пользователя, добавив в него поля:

1. Баланс в денежных единицах (с плавающей точкой, не может быть меньше 0.0);
2. Баланс в бонусах (с плавающей точкой, не может быть меньше 0.0);
3. Персональную скидку (с плавающей точкой, от 0.01 до 1.00, где 0.01 — скидка 99%, а 1.00 — скидка 0%);
4. Разрешённую долю оплаты бонусами (с плавающей точкой, от 0.0 до 0.5, где 0.0 — это 0%, и 0.5 — это 50% итоговой цены можно оплатить бонусами);
5. Совершённые Покупки — Список Товарных Позиций, отражающий приобретения Покупателя.

Нужно реализовать сценарии с успешной и неуспешной оплатой покупок покупателем (ниже — успешный сценарий с описанием возможных действий на случай неуспешного — п. 12–13):

1. Пользователь пополнил баланс на 10000 условных единиц денег (УЕ).
2. Пользователю подарили 10% от суммы бонусами.
3. Пользователю разрешили оплачивать до 10% итоговой стоимости Корзины его бонусами.
4. Пользователю предоставили персональную скидку 5%.
5. Пользователь Заказывает 3 Товара «Тарелка» стоимостью 1000 УЕ, 2 Товара «Прибор» стоимостью 2000 УЕ.
6. Заказанные Товары попадают в Корзину Покупателя в виде Товарных Позиций.
7. Корзина Покупателя возвращает Расчётную Цену с учётом персональной скидки.
8. Пользователю показывают итоговую цену товаров в корзине с учётом Персональной Скидки и Размер Скидки в Деньгах, вызвав методы Расчётной Цены «покажи итоговую цену» и «покажи размер скидки в деньгах».
9. Пользователь пытается оплатить итоговую цену бонусами настолько, насколько ему позволяет Разрешённая доля оплаты бонусами, остальное оплачивает из личного баланса в УЕ, передавая Состояние Баланса Покупателя и Расчётную Цену в сервисный метод оплаты (оплата бонусами и деньгами совершается в одном вызове метода оплаты!).
10. Если оплата возможна (денег и бонусов хватает на оплату Корзины со скидкой) — баланс пользователя приводится в соответствие с состоянием структуры Состояние Баланса Покупателя после проведения оплаты, а к списку Совершённых Покупок добавляется список Товарных Позиций из Корзины Покупателя.
11. Корзина Покупателя опустошается — в ней 0 товаров с итоговой ценой 0 УЕ.
12. Если оплата невозможна — Корзина остаётся в исходном состоянии (Товарные Позиции на месте), баланс Покупателя в УЕ и бонусах — остаётся в исходном состоянии (списаний не производится). На экране видим сообщение о нехватке средств на балансе.
13. После пополнения на необходимую сумму баланса Пользователя — пробуем провести успешную оплату.

### Задачи Лидера группы

1. собирает все части в один проект (по одному классу/структуре на файл);
2. пишет точку входа (`Main` или программы верхнего уровня), где:
   - создаёт покупателя и 2–3 заказа;
   - показывает разницу между копированием ссылки и значения;
   - выводит отчёты об успешных и не успешных операциях оплаты покупок;
   - демонстрирует `with` для Состояния Баланса Покупателя после успешной оплаты покупок;
3. проверяет, что копии не портят оригинальные данные;
4. оформляет отчёт.

### Рекомендации по выполнению (общие для проекта)

1. Один тип — один файл.
2. Пространства имён: типы проекта разместите в отдельном пространстве имён, например `Marketplace.Domain`, а сервис — в `Marketplace.Services`. Используйте файловое пространство имён (`namespace Marketplace.Domain;`).
3. Не делайте изменяемых структур без необходимости. Если структура может быть `readonly` — сделайте её `readonly`.
4. Проверяйте данные в конструкторах: отрицательная сумма, пустое имя, `null` вместо клиента — это ошибки, а не «потом разберёмся».
5. Обсуждайте общие решения: названия типов и методов должны быть одинаковыми во всех частях, чтобы компоновка не превратилась в переделку.
6. Каждый участник пишет либо небольшой класс, либо как минимум один серьёзный публичный метод сервиса с 1–2 приватными методами внутри.

## 3. Структура проекта и распределение задач

Проект собран в одну папку `Homework_2`, внутри которой каждая часть лежит в папке своего автора. Это позволило сразу видеть, кто за какой тип отвечает, и не смешивать чужие файлы при компоновке. Все файлы попадают в одну сборку, поэтому папки — это только организация исходников, на пространства имён они не влияют.

```text
Homework_2/
├── Marketplace.csproj
├── Lead/                    — файлы Лидера группы
│   ├── Contact.cs
│   ├── OrderService.cs
│   ├── PaymentResult.cs
│   └── Program.cs
├── Viola/                   — файлы участника Viola
│   ├── BalanceState.cs
│   └── UserProfile.cs
└── Viva/                    — файлы участника Viva
    ├── CalculatedPrice.cs
    ├── Product.cs
    ├── ProductPosition.cs
    ├── User.cs
    └── UserBasket.cs
```

| Задача ТЗ | Файл | Исполнитель |
|---|---|---|
| 1. Класс Товара | `Viva/Product.cs` | Viva |
| 2. Класс Товарной Позиции | `Viva/ProductPosition.cs` | Viva |
| 3. Класс Корзины Пользователя | `Viva/UserBasket.cs` | Viva |
| 4. Структура Расчётной Цены | `Viva/CalculatedPrice.cs` | Viva |
| 5. Структура Состояния Баланса | `Viola/BalanceState.cs` | Viola |
| Дополнение Профиля Пользователя | `Viola/UserProfile.cs` | Viola |
| Класс Пользователя | `Viva/User.cs` | Viva |
| Класс Контакта | `Lead/Contact.cs` | Лидер |
| Сервис оплаты | `Lead/OrderService.cs` | Лидер |
| Результат операции оплаты | `Lead/PaymentResult.cs` | Лидер |
| Точка входа и сценарии | `Lead/Program.cs` | Лидер |

**Решение по организации.** Файлы сгруппированы по авторам, а не по слоям (`Domain` / `Services`), потому что так проще было собирать части и видеть вклад каждого. Пространства имён при этом соблюдены по ТЗ: типы предметной области — `Marketplace.Domain`, сервис — `Marketplace.Services`.

## 4. Решения по подзаданиям

### 4.1. Класс Товара — `Viva/Product.cs`

Класс хранит ID, название и цену за единицу. ID присваивается в конструкторе из статического счётчика и доступен только для чтения, поэтому идентичность товара не меняется при изменении названия или цены. Название нормализуется (`Trim` и заглавная первая буква), пустое название и отрицательная цена отклоняются с исключением — по требованию п. 4 рекомендаций.

Использована `char.ToUpperInvariant`, а не `char.ToUpper`: преобразование с учётом текущей культуры дало бы разный результат в разных локалях (например, турецкая «i» → «İ»).

Цена хранится в `decimal`, а не в `double`: деньги не должны накапливать двоичную погрешность при суммировании позиций.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога: ID, название, цена за единицу.
/// </summary>
public class Product
{
    private static int counter;
    private string name = string.Empty;
    private decimal price;

    public int Id { get; }

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Название товара не может быть пустым.", nameof(Name));

            string text = value.Trim();
            name = char.ToUpperInvariant(text[0]) + text.Substring(1);
        }
    }

    public decimal Price
    {
        get => price;
        set
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(nameof(Price), "Цена не может быть отрицательной.");

            price = value;
        }
    }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
        Id = checked(++counter);
    }

    public override string ToString() => $"Товар #{Id} \"{Name}\", {Price:F2} УЕ";
}
```
### 4.2. Класс Товарной Позиции — `Viva/ProductPosition.cs`

Позиция хранит ID позиции, ID товара и количество. Проверки выполняются и в конструкторе, и в сеттере количества: позицию нельзя создать или изменить на некорректное количество.

Ключевое решение — метод `Copy()` на базе `MemberwiseClone`. Класс ссылочный, поэтому при передаче позиции в корзину без копии внешний код сохранял бы ссылку на тот же объект и мог изменить состав корзины в обход её методов. Копирование сохраняет `Id` позиции, так как все поля значимые и копия полностью независима.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Товарная позиция в корзине: ID позиции, ID товара, количество единиц.
/// </summary>
public class ProductPosition
{
    private static int counter;
    private int quantity;

    public int Id { get; }
    public int ProductId { get; }

    public int Quantity
    {
        get => quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Количество должно быть больше нуля.");

            quantity = value;
        }
    }

    public ProductPosition(int productId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "ID товара должен быть больше нуля.");

        ProductId = productId;
        Quantity = quantity;
        Id = checked(++counter);
    }

    // Все поля имеют типы значений: копия независима и сохраняет ID позиции.
    internal ProductPosition Copy() => (ProductPosition)MemberwiseClone();

    public override string ToString() => $"Позиция #{Id}: товар {ProductId} x {Quantity}";
}
```
### 4.3. Класс Корзины Пользователя — `Viva/UserBasket.cs`

Корзина хранит ID, ID пользователя и список товарных позиций. Список закрыт: наружу он отдаётся только копией через `GetPositions()`, а добавление идёт через `AddPosition`, который тоже копирует позицию. Так корзина остаётся единственным владельцем своего состояния.

Метод `CalculatePrice` принимает каталог товаров и множитель скидки. Каталог превращается в словарь по ID, что даёт две проверки сразу: повторяющийся ID товара и отсутствие нужного товара. Множитель проверяется даже для пустой корзины, поэтому некорректная скидка не «проваливается» молча на пустом списке.

Добавлены `Count` и `IsEmpty`, чтобы проверять пустоту без копирования всего списка, и `SetQuantity` для изменения количества уже добавленной позиции.

```csharp
using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>
/// Корзина покупателя: ID, ID пользователя и список товарных позиций.
/// </summary>
public class UserBasket
{
    private static int counter;
    private readonly List<ProductPosition> positions = new();

    public int Id { get; }
    public int UserId { get; }
    public int Count => positions.Count;
    public bool IsEmpty => positions.Count == 0;

    public UserBasket(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId), "ID пользователя должен быть больше нуля.");

        UserId = userId;
        Id = checked(++counter);
    }

    public void AddPosition(ProductPosition position)
    {
        if (position == null)
            throw new ArgumentNullException(nameof(position));

        if (positions.Exists(item => item.Id == position.Id))
            throw new ArgumentException("Позиция уже добавлена в корзину.", nameof(position));

        positions.Add(position.Copy());
    }

    /// <summary>
    /// Возвращает позиции корзины. Копируются и список, и позиции:
    /// историю покупок можно хранить независимо от корзины.
    /// </summary>
    public List<ProductPosition> GetPositions() => positions.ConvertAll(position => position.Copy());

    public bool SetQuantity(int positionId, int quantity)
    {
        if (positionId <= 0)
            return false;

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть больше нуля.");

        ProductPosition? position = positions.Find(item => item.Id == positionId);
        if (position == null)
            return false;

        position.Quantity = quantity;
        return true;
    }

    public bool RemovePosition(int positionId)
    {
        if (positionId <= 0)
            return false;

        return positions.RemoveAll(position => position.Id == positionId) > 0;
    }

    /// <summary>Вызывать после успешной оплаты. При отказе сервис не должен очищать корзину.</summary>
    public void Clear() => positions.Clear();

    public CalculatedPrice CalculatePrice(IEnumerable<Product> products, decimal discountMultiplier)
    {
        if (products == null)
            throw new ArgumentNullException(nameof(products));

        // Проверяем скидку даже для пустой корзины.
        CalculatedPrice.ValidateDiscountMultiplier(discountMultiplier);

        if (positions.Count == 0)
            return new CalculatedPrice(0m, discountMultiplier);

        var catalog = new Dictionary<int, Product>();
        foreach (Product product in products)
        {
            if (product == null)
                throw new ArgumentException("Каталог не должен содержать null.", nameof(products));

            if (!catalog.TryAdd(product.Id, product))
                throw new ArgumentException("В каталоге повторяется ID товара.", nameof(products));
        }

        decimal originalPrice = 0m;
        foreach (ProductPosition position in positions)
        {
            if (!catalog.TryGetValue(position.ProductId, out Product? product))
                throw new InvalidOperationException("В каталоге нет товара с ID " + position.ProductId + ".");

            originalPrice += product.Price * position.Quantity;
        }

        return new CalculatedPrice(originalPrice, discountMultiplier);
    }

    public override string ToString() => $"Корзина #{Id} пользователя {UserId}: позиций {Count}";
}
```
### 4.4. Структура Расчётной Цены — `Viva/CalculatedPrice.cs`

Структура объявлена `readonly`: расчётная цена — это снимок результата, менять его поля после создания не нужно. Значения проверяются в конструкторе, множитель ограничен диапазоном 0.01–1.00, где 1.00 означает скидку 0%.

Округление денег задано явно: `Math.Round(..., 2, MidpointRounding.AwayFromZero)` для исходной цены и для итоговой. Без единого правила суммы в тестах расходились бы на копейки. Исходная цена тоже округляется при создании, поэтому `DiscountAmount` всегда получается ровно с двумя знаками.

Размер скидки считается как `OriginalPrice - FinalPrice`, поэтому скидка не может оказаться отрицательной: множитель не превышает 1.00.

Метод `ValidateDiscountMultiplier` вынесен в `internal`, чтобы корзина могла проверить скидку до проверки на пустоту.

Дополнительно: `decimal` не может быть `NaN` или бесконечностью, поэтому проверка диапазона здесь работает честно — в отличие от полей типа `double` в профиле, где обычное сравнение пропускает `NaN`.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Расчётная цена корзины: исходная цена, множитель скидки (0.01–1.00) и размер скидки в деньгах.
/// Неизменяемая структура.
/// </summary>
public readonly struct CalculatedPrice
{
    public decimal OriginalPrice { get; }
    public decimal DiscountMultiplier { get; }

    public decimal FinalPrice => Math.Round(OriginalPrice * DiscountMultiplier, 2, MidpointRounding.AwayFromZero);

    public decimal DiscountAmount => OriginalPrice - FinalPrice;

    public CalculatedPrice(decimal originalPrice, decimal discountMultiplier)
    {
        if (originalPrice < 0m)
            throw new ArgumentOutOfRangeException(nameof(originalPrice), "Исходная цена не может быть отрицательной.");

        ValidateDiscountMultiplier(discountMultiplier);

        OriginalPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
        DiscountMultiplier = discountMultiplier;
    }

    /// <summary>
    /// Проверка множителя скидки. Вынесена, чтобы корзина могла проверить скидку
    /// даже при пустом списке позиций.
    /// </summary>
    internal static void ValidateDiscountMultiplier(decimal discountMultiplier)
    {
        if (discountMultiplier < 0.01m || discountMultiplier > 1m)
            throw new ArgumentOutOfRangeException(nameof(discountMultiplier),
                "Множитель скидки должен быть от 0.01 до 1.00.");
    }

    public override string ToString() =>
        $"Исходная цена: {OriginalPrice:F2} УЕ, скидка: {DiscountAmount:F2} УЕ, к оплате: {FinalPrice:F2} УЕ";
}
```
### 4.5. Структура Состояния Баланса — `Viola/BalanceState.cs`

Структура объявлена как `readonly record struct`. Это даёт сразу три свойства:

1. она значимая, поэтому копируется по значению;
2. она неизменяемая, поэтому после оплаты нельзя «подправить» баланс на месте;
3. она поддерживает выражение `with`, что требуется по ТЗ для демонстрации.

Валидация перенесена в `init`-аксессоры, а не только в конструктор. Это принципиально для `with`: если проверять значения лишь в конструкторе, то `state with { Money = -5 }` обошёл бы все проверки и создал некорректное состояние. С проверкой в `init` такой вызов бросает исключение.

Проверки используют `double.IsFinite`, поэтому `NaN` и бесконечность отсекаются. Обычное сравнение `value < 0.0` их пропускает: любое сравнение с `NaN` даёт `false`.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Состояние баланса покупателя: бонусы, деньги и разрешённая доля оплаты бонусами.
/// Неизменяемая структура (readonly record struct), поэтому поддерживает выражение with
/// и при этом не позволяет изменить состояние на месте.
/// </summary>
public readonly record struct BalanceState
{
    private readonly double bonuses;
    private readonly double money;
    private readonly double bonusPaymentRatio;

    public double Bonuses
    {
        get => bonuses;
        init
        {
            if (!double.IsFinite(value) || value < 0.0)
                throw new ArgumentOutOfRangeException(nameof(Bonuses), "Количество бонусов не может быть отрицательным.");

            bonuses = value;
        }
    }

    public double Money
    {
        get => money;
        init
        {
            if (!double.IsFinite(value) || value < 0.0)
                throw new ArgumentOutOfRangeException(nameof(Money), "Количество денежных единиц не может быть отрицательным.");

            money = value;
        }
    }

    /// <summary>Доля итоговой цены, которую можно оплатить бонусами: от 0.0 до 0.5.</summary>
    public double BonusPaymentRatio
    {
        get => bonusPaymentRatio;
        init
        {
            if (!double.IsFinite(value) || value < 0.0 || value > 0.5)
                throw new ArgumentOutOfRangeException(nameof(BonusPaymentRatio),
                    "Доля оплаты бонусами должна быть от 0.0 до 0.5.");

            bonusPaymentRatio = value;
        }
    }

    public BalanceState(double bonuses, double money, double bonusPaymentRatio)
    {
        Bonuses = bonuses;
        Money = money;
        BonusPaymentRatio = bonusPaymentRatio;
    }

    public override string ToString() =>
        $"Баланс: {Money:F2} УЕ, {Bonuses:F2} бонусов. Возможная доля оплаты бонусами: {BonusPaymentRatio:P0}";
}
```
### 4.6. Дополнение Профиля Пользователя — `Viola/UserProfile.cs`

Класс из Домашнего Задания 1 дополнен пятью полями по ТЗ: баланс в деньгах, баланс в бонусах, персональная скидка, разрешённая доля оплаты бонусами и список совершённых покупок.

Денежные поля имеют тип `double` — ТЗ прямо требует «с плавающей точкой». Суммы при этом считаются в `decimal` внутри корзины и структуры цены, а приведение выполняется только на границе, в сервисе оплаты.

Список покупок отдаётся наружу как `IReadOnlyList<ProductPosition>`, а `AddPurchases` копирует позиции. Это защищает историю: изменение позиции в корзине после оплаты не перепишет уже совершённую покупку.

Проверки суммы и скидки идут через `!double.IsFinite(...) || ...`, чтобы отсечь `NaN`. Иначе `AddMoney(double.NaN)` прошёл бы проверку `amount <= 0.0` и испортил баланс.

Деконструктор с четырьмя параметрами добавлен рядом с прежним деконструктором из трёх параметров: разная арность не создаёт конфликта перегрузки.

```csharp
using System;
using System.Collections.Generic;

namespace Marketplace.Domain;

/// <summary>
/// Профиль пользователя: контакты, балансы, скидка и история покупок.
/// Денежные поля имеют тип double — по требованию ТЗ («с плавающей точкой»).
/// </summary>
public class UserProfile
{
    private readonly List<Contact> _contacts = new();
    private readonly List<ProductPosition> _purchases = new();

    public User User { get; }

    public IReadOnlyList<Contact> Contacts => _contacts;
    public IReadOnlyList<ProductPosition> Purchases => _purchases;

    /// <summary>Баланс в денежных единицах. Не может быть отрицательным.</summary>
    public double MoneyBalance { get; private set; }

    /// <summary>Баланс в бонусах. Не может быть отрицательным.</summary>
    public double BonusBalance { get; private set; }

    /// <summary>Персональная скидка: множитель от 0.01 до 1.00, где 1.00 — скидка 0%.</summary>
    public double PersonalDiscount { get; private set; }

    /// <summary>Разрешённая доля оплаты бонусами: от 0.0 до 0.5.</summary>
    public double AllowedBonusRatio { get; private set; }

    public UserProfile(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));

        MoneyBalance = 0.0;
        BonusBalance = 0.0;
        PersonalDiscount = 1.00;   // 1.00 — это 0% скидки
        AllowedBonusRatio = 0.0;
    }

    // ---------- Контакты (ДЗ 1) ----------

    public void AddContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        if (_contacts.Contains(contact))
            throw new ArgumentException("Этот контакт уже добавлен в профиль.", nameof(contact));

        _contacts.Add(contact);
    }

    public bool RemoveContact(int contactId)
    {
        if (contactId <= 0)
            return false;

        var contactToRemove = _contacts.Find(c => c.Id == contactId);
        return contactToRemove != null && _contacts.Remove(contactToRemove);
    }

    public void Deconstruct(out int userId, out string userFullName, out string contactsInfo)
    {
        userId = User.Id;
        userFullName = User.FullName;

        var sb = new System.Text.StringBuilder();

        if (Contacts.Count == 0)
        {
            sb.Append("Контактов нет.");
        }
        else
        {
            foreach (var contact in Contacts)
            {
                var (_, channelId, identifier) = contact;
                sb.AppendLine($"Контакт канала {channelId}: {identifier}");
            }
        }

        contactsInfo = sb.ToString();
    }

    /// <summary>Деконструктор для денежных вопросов.</summary>
    public void Deconstruct(out double money, out double bonuses, out double discountMultiplier, out int purchaseCount)
    {
        money = MoneyBalance;
        bonuses = BonusBalance;
        discountMultiplier = PersonalDiscount;
        purchaseCount = Purchases.Count;
    }

    // ---------- Балансы ----------

    public void AddMoney(double amount)
    {
        // Проверка через отрицание нужна, чтобы отсечь NaN: NaN не меньше нуля и не больше нуля.
        if (!double.IsFinite(amount) || amount <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма пополнения должна быть больше нуля.");

        MoneyBalance += amount;
    }

    public void AddBonuses(double amount)
    {
        if (!double.IsFinite(amount) || amount <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма бонусов для пополнения должна быть больше нуля.");

        BonusBalance += amount;
    }

    public void SetPersonalDiscount(double discount)
    {
        if (!double.IsFinite(discount) || discount < 0.01 || discount > 1.00)
            throw new ArgumentOutOfRangeException(nameof(discount), "Скидка должна быть в диапазоне от 0.01 до 1.00.");

        PersonalDiscount = discount;
    }

    public void SetAllowedBonusRatio(double ratio)
    {
        if (!double.IsFinite(ratio) || ratio < 0.0 || ratio > 0.5)
            throw new ArgumentOutOfRangeException(nameof(ratio), "Доля оплаты бонусами должна быть от 0.0 до 0.5.");

        AllowedBonusRatio = ratio;
    }

    /// <summary>
    /// Применяет новое состояние баланса после оплаты. Вызывается сервисом оплаты.
    /// Рассчитывает на корректность BalanceState — проверки выполняются в его конструкторе.
    /// </summary>
    public void ApplyNewBalance(BalanceState newState)
    {
        MoneyBalance = newState.Money;
        BonusBalance = newState.Bonuses;
        AllowedBonusRatio = newState.BonusPaymentRatio;
    }

    // ---------- История покупок ----------

    /// <summary>
    /// Добавляет купленные позиции в историю. Позиции копируются, чтобы история
    /// не зависела от корзины и не могла быть изменена извне.
    /// </summary>
    public void AddPurchases(IEnumerable<ProductPosition> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var buffer = new List<ProductPosition>();

        foreach (var item in items)
        {
            if (item == null)
                throw new ArgumentException("Список покупок не должен содержать null.", nameof(items));

            buffer.Add(item.Copy());
        }

        _purchases.AddRange(buffer);
    }

    public override string ToString() =>
        $"{User.FullName}: {MoneyBalance:F2} УЕ, {BonusBalance:F2} бонусов, скидка {PersonalDiscount:P0}, покупок: {Purchases.Count}";
}
```
### 4.7. Пользователь и Контакт — `Viva/User.cs`, `Lead/Contact.cs`

Эти два вспомогательных типа написаны заново под текущую задачу. `User` хранит ID и полное имя, `Contact` — ID, ID канала и идентификатор в канале; деконструктор контакта используется в `UserProfile.Deconstruct` из Домашнего Задания 1. Оба типа содержат только то, что нужно профилю и сценариям, и валидируют пустые значения в конструкторе.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Пользователь платформы. Написан заново под текущую задачу:
/// профилю нужны только ID и полное имя.
/// </summary>
public class User
{
    private static int counter;
    private string fullName = string.Empty;

    public int Id { get; }

    public string FullName
    {
        get => fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Имя пользователя не может быть пустым.", nameof(FullName));

            fullName = value.Trim();
        }
    }

    public User(string fullName)
    {
        FullName = fullName;
        Id = checked(++counter);
    }

    public override string ToString() => $"Пользователь #{Id} {FullName}";
}
```

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Контакт пользователя. Написан заново под текущую задачу:
/// деконструктор отдаёт ID, ID канала и идентификатор —
/// ровно то, что использует UserProfile.Deconstruct.
/// </summary>
public class Contact
{
    private static int counter;
    private string identifier = string.Empty;

    public int Id { get; }
    public int ChannelId { get; }

    public string Identifier
    {
        get => identifier;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Идентификатор контакта не может быть пустым.", nameof(Identifier));

            identifier = value.Trim();
        }
    }

    public Contact(int channelId, string identifier)
    {
        if (channelId <= 0)
            throw new ArgumentOutOfRangeException(nameof(channelId), "ID канала должен быть больше нуля.");

        ChannelId = channelId;
        Identifier = identifier;
        Id = checked(++counter);
    }

    public void Deconstruct(out int id, out int channelId, out string identifier)
    {
        id = Id;
        channelId = ChannelId;
        identifier = Identifier;
    }

    public override string ToString() => $"Контакт #{Id}: канал {ChannelId}, {Identifier}";
}
```
### 4.8. Сервис оплаты — `Lead/OrderService.cs`

Сервис реализует п. 9–13 ТЗ. Оплата бонусами и деньгами выполняется в одном вызове `Pay`, как требует ТЗ. Метод принимает `BalanceState` и `CalculatedPrice` — то есть ровно те структуры, которые описаны в задании.

Разделение цены на бонусную и денежную части вынесено в приватный метод `SplitPayment`. Бонусами закрывается минимум из трёх величин: разрешённой доли от итоговой цены, остатка бонусов и самой цены. Остаток оплачивается деньгами.

Второй публичный метод `Checkout` собирает полный сценарий покупки: расчёт цены по персональной скидке, оплата, и только при успехе — обновление профиля, добавление покупок в историю и очистка корзины. При отказе ни одно из этих действий не выполняется, поэтому корзина и баланс остаются в исходном состоянии (п. 12 ТЗ).

Новое состояние баланса собирается выражением `with`, а исходная структура остаётся неизменной — это и есть демонстрация, требуемая ТЗ.

```csharp
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
    /// Оплата покупки: часть итоговой цены закрывается бонусами в пределах
    /// разрешённой доли, остаток — деньгами с личного баланса.
    /// При нехватке средств состояние баланса не меняется.
    /// </summary>
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

    /// <summary>
    /// Полный сценарий покупки: расчёт цены по персональной скидке, оплата,
    /// при успехе — обновление баланса, добавление покупок в историю и очистка корзины.
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
```
### 4.9. Результат операции оплаты — `Lead/PaymentResult.cs`

Для отчёта об операциях нужен единый объект результата. Он также объявлен `readonly record struct`: результат — это значение, а не сущность с собственной жизнью, и его удобно печатать в таблице.

```csharp
using System;

namespace Marketplace.Services;

using Marketplace.Domain;

/// <summary>
/// Результат попытки оплаты. Неизменяемая структура.
/// </summary>
public readonly record struct PaymentResult
{
    /// <summary>Признак успешной оплаты.</summary>
    public bool Success { get; init; }

    /// <summary>Сообщение для вывода покупателю.</summary>
    public string Message { get; init; }

    /// <summary>Сколько бонусов списано.</summary>
    public decimal BonusesSpent { get; init; }

    /// <summary>Сколько денежных единиц списано.</summary>
    public decimal MoneySpent { get; init; }

    /// <summary>Состояние баланса после операции. При отказе совпадает с исходным.</summary>
    public BalanceState NewBalance { get; init; }

    /// <summary>Расчётная цена, по которой совершалась попытка оплаты.</summary>
    public CalculatedPrice Price { get; init; }
}
```
### 4.10. Точка входа и сценарии — `Lead/Program.cs`

`Main` последовательно проходит сценарий ТЗ и дополнительно демонстрирует требования Лидера группы: разницу между копированием ссылки и значения, отчёт об успешных и неуспешных оплатах, `with` для состояния баланса и работу проверок на некорректных данных.

```csharp
using System;
using System.Collections.Generic;

using Marketplace.Domain;
using Marketplace.Services;

namespace Marketplace;

/// <summary>
/// Точка входа: демонстрация сценариев Задания 3.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Section("1. Подготовка каталога и покупателя");

        var plate = new Product("Тарелка", 1000m);
        var device = new Product("Прибор", 2000m);
        var catalog = new List<Product> { plate, device };

        Console.WriteLine($"Созданы товары: {plate}, {device}");

        var user = new User("Иванов Иван");
        var profile = new UserProfile(user);
        profile.AddContact(new Contact(1, "@ivanov"));

        // По ТЗ: пополнение 10000 УЕ, бонусы 10% от суммы, доля бонусов 10%, скидка 5%.
        profile.AddMoney(10_000.0);
        profile.AddBonuses(10_000.0 * 0.10);
        profile.SetAllowedBonusRatio(0.10);
        profile.SetPersonalDiscount(0.95);

        var (money, bonuses, discount, purchaseCount) = profile;
        Console.WriteLine($"Покупатель: {profile.User.FullName}");
        Console.WriteLine($"  Баланс: {money:F2} УЕ, бонусов {bonuses:F2}, множитель скидки {discount:F2}, покупок {purchaseCount}");

        Section("2. Заказ товаров и наполнение корзины");

        var basket = new UserBasket(profile.User.Id);

        // 3 товара "Тарелка" по 1000 УЕ и 2 товара "Прибор" по 2000 УЕ.
        basket.AddPosition(new ProductPosition(plate.Id, 3));
        basket.AddPosition(new ProductPosition(device.Id, 2));

        Console.WriteLine($"В корзину добавлено позиций: {basket.Count}");

        Section("3. Расчётная цена с учётом персональной скидки");

        CalculatedPrice price = basket.CalculatePrice(catalog, (decimal)profile.PersonalDiscount);
        Console.WriteLine($"Показать итоговую цену: {price.FinalPrice:F2} УЕ");
        Console.WriteLine($"Показать размер скидки в деньгах: {price.DiscountAmount:F2} УЕ");
        Console.WriteLine($"({price})");

        Section("4. Копирование ссылки и значения");

        // Класс — ссылочный тип: две переменные указывают на один объект.
        var positionA = new ProductPosition(plate.Id, 1);
        var positionB = positionA;
        positionB.Quantity = 5;
        Console.WriteLine($"Класс: positionA.Quantity = {positionA.Quantity}, positionB.Quantity = {positionB.Quantity} — объект один.");

        // Структура — значимый тип: копия независима, оригинал не меняется.
        var priceBefore = new CalculatedPrice(1000m, 0.90m);
        CalculatedPrice priceCopy = priceBefore;
        priceCopy = new CalculatedPrice(500m, 0.90m);
        Console.WriteLine($"Структура: оригинал {priceBefore.FinalPrice:F2} УЕ, копия {priceCopy.FinalPrice:F2} УЕ — оригинал не изменился.");

        // Копирование позиции внутри корзины: изменение копии не портит оригинал.
        // Используется отдельная корзина, чтобы не менять состав основной.
        var demoBasket = new UserBasket(profile.User.Id);
        var original = new ProductPosition(device.Id, 2);
        demoBasket.AddPosition(original);
        original.Quantity = 99;
        Console.WriteLine($"Корзина хранит копию: в корзине позиция с Quantity = "
                          + $"{demoBasket.GetPositions().Find(p => p.Id == original.Id)!.Quantity}, оригинал изменён на {original.Quantity}.");

        Section("5. Неуспешная оплата: нехватка средств");

        // Скидка убрана, дорогая корзина, денег заведомо не хватает.
        var poorUser = new User("Петров Пётр");
        var poorProfile = new UserProfile(poorUser);
        poorProfile.AddMoney(500.0);
        poorProfile.SetPersonalDiscount(1.00);

        var poorBasket = new UserBasket(poorProfile.User.Id);
        poorBasket.AddPosition(new ProductPosition(device.Id, 3));

        int positionsBefore = poorBasket.Count;
        var poorService = new OrderService();
        PaymentResult failed = poorService.Checkout(poorProfile, poorBasket, catalog);

        Console.WriteLine($"Успех: {failed.Success}");
        Console.WriteLine($"Сообщение: {failed.Message}");
        Console.WriteLine($"Корзина сохранена: {poorBasket.Count} позиций (было {positionsBefore}); баланс: {poorProfile.MoneyBalance:F2} УЕ");

        Section("6. Пополнение баланса и успешная оплата");

        decimal missing = failed.Price.FinalPrice - (decimal)poorProfile.MoneyBalance;
        Console.WriteLine($"Не хватало {missing:F2} УЕ — пополняем.");
        poorProfile.AddMoney((double)missing);

        PaymentResult success = poorService.Checkout(poorProfile, poorBasket, catalog);
        Console.WriteLine($"Успех: {success.Success}");
        Console.WriteLine($"Сообщение: {success.Message}");
        Console.WriteLine($"Корзина опустошена: {poorBasket.IsEmpty}, итоговая цена пустой корзины: "
                          + $"{poorBasket.CalculatePrice(catalog, 1.00m).FinalPrice:F2} УЕ");
        Console.WriteLine($"Баланс после оплаты: {poorProfile.MoneyBalance:F2} УЕ, бонусов {poorProfile.BonusBalance:F2}");
        Console.WriteLine($"Покупок в истории: {poorProfile.Purchases.Count}");

        Section("7. Демонстрация with для Состояния Баланса");

        BalanceState stateBefore = new BalanceState(bonuses: 500.0, money: 2000.0, bonusPaymentRatio: 0.10);
        BalanceState stateAfter = stateBefore with { Money = 1500.0 };

        Console.WriteLine($"До:    {stateBefore}");
        Console.WriteLine($"После: {stateAfter}");
        Console.WriteLine($"Исходное состояние не изменилось: {stateBefore.Money:F2} УЕ");

        Section("8. Оплата основной корзиной покупателя");

        PaymentResult mainPayment = new OrderService().Checkout(profile, basket, catalog);
        Console.WriteLine($"Успех: {mainPayment.Success}");
        Console.WriteLine($"Сообщение: {mainPayment.Message}");
        Console.WriteLine($"Баланс: {profile.MoneyBalance:F2} УЕ, бонусов {profile.BonusBalance:F2}");
        Console.WriteLine($"Корзина: {basket.Count} позиций, пуста: {basket.IsEmpty}");
        Console.WriteLine($"Покупок в истории: {profile.Purchases.Count}");

        Section("9. Отчёт об операциях оплаты");

        PrintReport(new[] { failed, success, mainPayment });

        Section("10. Проверка валидации (ожидаем исключения)");

        ExpectThrow("Пустое название товара", () => new Product("   ", 100m));
        ExpectThrow("Отрицательная цена", () => new Product("Товар", -1m));
        ExpectThrow("Нулевое количество", () => new ProductPosition(plate.Id, 0));
        ExpectThrow("Скидка 0.00", () => profile.SetPersonalDiscount(0.00));
        ExpectThrow("Доля бонусов 0.75", () => profile.SetAllowedBonusRatio(0.75));
        ExpectThrow("Пополнение на NaN", () => profile.AddMoney(double.NaN));
        ExpectThrow("Множитель скидки 1.5", () => new CalculatedPrice(100m, 1.5m));
        ExpectThrow("Отрицательный баланс бонусов", () => new BalanceState(-1.0, 0.0, 0.0));
        ExpectThrow("Нет товара в каталоге", () =>
        {
            var isolated = new UserBasket(profile.User.Id);
            isolated.AddPosition(new ProductPosition(device.Id, 1));
            isolated.CalculatePrice(new List<Product> { plate }, 1.00m);
        });
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 70));
    }

    private static void PrintReport(IEnumerable<PaymentResult> results)
    {
        Console.WriteLine($"{"Успех",-8}{"Итоговая цена",15}{"Бонусы",12}{"Деньги",12}  Сообщение");
        Console.WriteLine(new string('-', 100));

        foreach (var result in results)
        {
            Console.WriteLine($"{result.Success,-8}{result.Price.FinalPrice,15:F2}{result.BonusesSpent,12:F2}"
                              + $"{result.MoneySpent,12:F2}  {result.Message}");
        }
    }

    private static void ExpectThrow(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {title}: исключения не было");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OK] {title}: {ex.GetType().Name} — {ex.Message}");
        }
    }
}
```
## 5. Результат выполнения программы

Ниже — фактический вывод программы (сокращённо, все секции сохранены).

```text
1. Подготовка каталога и покупателя
Созданы товары: Товар #1 "Тарелка", 1000.00 УЕ, Товар #2 "Прибор", 2000.00 УЕ
Покупатель: Иванов Иван
  Баланс: 10000.00 УЕ, бонусов 1000.00, множитель скидки 0.95, покупок 0

2. Заказ товаров и наполнение корзины
В корзину добавлено позиций: 2

3. Расчётная цена с учётом персональной скидки
Показать итоговую цену: 6650.00 УЕ
Показать размер скидки в деньгах: 350.00 УЕ
(Исходная цена: 7000.00 УЕ, скидка: 350.00 УЕ, к оплате: 6650.00 УЕ)

4. Копирование ссылки и значения
Класс: positionA.Quantity = 5, positionB.Quantity = 5 — объект один.
Структура: оригинал 900.00 УЕ, копия 450.00 УЕ — оригинал не изменился.
Корзина хранит копию: в корзине позиция с Quantity = 2, оригинал изменён на 99.

5. Неуспешная оплата: нехватка средств
Успех: False
Сообщение: Недостаточно средств на балансе: не хватает 5500.00 УЕ.
Корзина сохранена: 1 позиций (было 1); баланс: 500.00 УЕ

6. Пополнение баланса и успешная оплата
Не хватало 5500.00 УЕ — пополняем.
Успех: True
Сообщение: Оплата прошла успешно. Списано денег: 6000.00 УЕ, бонусы не использовались.
Корзина опустошена: True, итоговая цена пустой корзины: 0.00 УЕ
Баланс после оплаты: 0.00 УЕ, бонусов 0.00
Покупок в истории: 1

7. Демонстрация with для Состояния Баланса
До:    Баланс: 2000.00 УЕ, 500.00 бонусов. Возможная доля оплаты бонусами: 10 %
После: Баланс: 1500.00 УЕ, 500.00 бонусов. Возможная доля оплаты бонусами: 10 %
Исходное состояние не изменилось: 2000.00 УЕ

8. Оплата основной корзиной покупателя
Успех: True
Сообщение: Оплата прошла успешно. Списано бонусов: 665.00, денег: 5985.00 УЕ.
Баланс: 4015.00 УЕ, бонусов 335.00
Корзина: 0 позиций, пуста: True
Покупок в истории: 2

9. Отчёт об операциях оплаты
Успех     Итоговая цена      Бонусы      Деньги  Сообщение
----------------------------------------------------------------------------------------------------
False           6000.00        0.00        0.00  Недостаточно средств на балансе: не хватает 5500.00 УЕ.
True            6000.00        0.00     6000.00  Оплата прошла успешно. Списано денег: 6000.00 УЕ, бонусы не использовались.
True            6650.00      665.00     5985.00  Оплата прошла успешно. Списано бонусов: 665.00, денег: 5985.00 УЕ.

10. Проверка валидации (ожидаем исключения)
[OK] Пустое название товара: ArgumentException — Название товара не может быть пустым. (Parameter 'Name')
[OK] Отрицательная цена: ArgumentOutOfRangeException — Цена не может быть отрицательной. (Parameter 'Price')
[OK] Нулевое количество: ArgumentOutOfRangeException — Количество должно быть больше нуля. (Parameter 'Quantity')
[OK] Скидка 0.00: ArgumentOutOfRangeException — Скидка должна быть в диапазоне от 0.01 до 1.00. (Parameter 'discount')
[OK] Доля бонусов 0.75: ArgumentOutOfRangeException — Доля оплаты бонусами должна быть от 0.0 до 0.5. (Parameter 'ratio')
[OK] Пополнение на NaN: ArgumentOutOfRangeException — Сумма пополнения должна быть больше нуля. (Parameter 'amount')
[OK] Множитель скидки 1.5: ArgumentOutOfRangeException — Множитель скидки должен быть от 0.01 до 1.00. (Parameter 'discountMultiplier')
[OK] Отрицательный баланс бонусов: ArgumentOutOfRangeException — Количество бонусов не может быть отрицательным. (Parameter 'Bonuses')
[OK] Нет товара в каталоге: InvalidOperationException — В каталоге нет товара с ID 2.
```

**Проверка арифметики сценария ТЗ:**

| Шаг | Расчёт | Результат |
|---|---|---|
| Исходная цена корзины | 3 × 1000 + 2 × 2000 | 7000.00 УЕ |
| Персональная скидка 5% (множитель 0.95) | 7000 × 0.95 | 6650.00 УЕ |
| Размер скидки в деньгах | 7000 − 6650 | 350.00 УЕ |
| Бонусами можно закрыть 10% | 6650 × 0.10 | 665.00 |
| Остаток деньгами | 6650 − 665 | 5985.00 УЕ |
| Баланс после оплаты | 10000 − 5985 | 4015.00 УЕ |
| Бонусы после оплаты | 1000 − 665 | 335.00 |

## 6. Вопросы, возникшие в ходе работы

Вопросов нету.

## 7. Выводы

### Что оказалось сложным

1. **Стыковка частей, написанных разными участниками.** Главная сложность — не сами классы, а договорённости между ними: имя класса товарной позиции, пространство имён, тип денег. Расхождение в одном имени (`CartItem` против `ProductPosition`) останавливает сборку всего проекта, хотя каждый класс по отдельности верен. Вывод: общие имена и типы надо фиксировать до начала работы, как и написано в п. 5 рекомендаций.

2. **Выбор между `double` и `decimal`.** ТЗ требует «плавающую точку» для балансов, но деньги в `double` накапливают погрешность. Решение — считать в `decimal` внутри расчёта и хранить `double` в профиле по требованию ТЗ, приводя тип только на границе.

3. **Валидация `NaN`.** Проверка `value < 0.0` выглядит корректной, но `NaN` проходит её насквозь. Это обнаружилось только при написании тестов на граничные значения.

4. **Атомарность неуспешной оплаты.** Требование «корзина и баланс остаются в исходном состоянии» легко нарушить: достаточно обновить баланс до проверки достаточности средств. Пришлось сначала считать результат, потом применять его.

5. **Округление денег.** Отдельное правило потребовалось и для исходной, и для итоговой суммы; при отсутствии единого правила суммы в отчётах расходились на копейки.

### Чем структуры отличаются от классов на практике

1. **Копирование.** Класс при присваивании копирует ссылку, поэтому две переменные указывают на один объект: изменение через одну видно через другую. Структура копирует всё значение целиком, и копия независима. Это хорошо видно в секции 4 вывода программы.

2. **Владение данными.** Ссылочный тип требует явного копирования на границах (`Copy()` в позиции, копия списка в корзине), иначе внутреннее состояние можно изменить в обход методов. Структуру копировать не нужно — она копируется сама.

3. **Изменяемость.** Структуру легко случайно изменить на копии, решив, что меняете оригинал. Поэтому в проекте обе структуры объявлены `readonly`: они описывают снимок результата, а не объект с поведением.

4. **Значения по умолчанию.** У класса `default` — это `null`, обращение к нему даёт `NullReferenceException`. У структуры `default` — валидное значение со всеми полями по умолчанию, поэтому `default(CalculatedPrice)` существует и не падает. Из-за этого пришлось явно решить, что означает пустая структура с нулевым множителем, и в отчёте это описано.

5. **Валидация и `init`.** В классе с сеттерами проверку можно поставить в сеттер. В `readonly` структуре сеттеров нет, поэтому проверки размещены в `init`-аксессорах — иначе `with` создавал бы некорректные значения в обход конструктора.

### Где в проекте использовано `with`

`with` применено в двух местах:

1. `Lead/OrderService.cs`, метод `Pay` — после успешной оплаты новое состояние собирается как `state with { Money = ..., Bonuses = ... }`. Исходное состояние остаётся неизменным, что позволяет показать разницу до и после.

2. `Lead/Program.cs`, секция 7 — явная демонстрация: `BalanceState stateAfter = stateBefore with { Money = 1500.0 };` с выводом обоих состояний и подтверждением, что исходное не изменилось.

`with` работает потому, что `BalanceState` объявлена как `readonly record struct`. Для обычной структуры без `record` выражение `with` в C# 10+ тоже доступно, но требует доступных для записи полей, что противоречит `readonly`.

### Итог

Все пять типов из ТЗ реализованы и собираются в один проект, сценарии успешной и неуспешной оплаты воспроизведены и проверены запуском, расчёты совпадают с ожидаемыми по ТЗ. Разделение на `Marketplace.Domain` и `Marketplace.Services` соблюдено, один тип — один файл, файлы сгруппированы по авторам.

## 8. Приложение: состав проекта

```text
Homework_2/
├── Marketplace.csproj
├── Results_Homework_2_README.md     — отчёт
├── Lead/
│   ├── Contact.cs                   — класс Контакта
│   ├── OrderService.cs              — сервис оплаты
│   ├── PaymentResult.cs             — результат операции оплаты
│   └── Program.cs                   — точка входа, сценарии, отчёты 
├── Viola/
│   ├── BalanceState.cs              — структура Состояния Баланса
│   └── UserProfile.cs               — Профиль Пользователя
└── Viva/
    ├── CalculatedPrice.cs           — структура Расчётной Цены
    ├── Product.cs                   — класс Товара
    ├── ProductPosition.cs           — класс Товарной Позиции
    ├── User.cs                      — класс Пользователя
    └── UserBasket.cs                — класс Корзины Пользователя
```

Проект собирается и запускается на без предупреждений:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
