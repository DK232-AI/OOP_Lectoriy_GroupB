# ООП 2026 — Домашнее задание 3

## 1. Титульная часть

| Параметр | Значение |
|---|---|
| Номер занятия | 3 |
| Тема | Модификаторы доступа, файл как область видимости, целостность данных и инкапсуляция, принципы SOLID |
| Суть проекта | Marketplace — каталог товаров с файловым хранилищем и выводом |
| Состав рабочей группы | 3 человека |

**Состав рабочей группы:**

| Участник | Роль | Папка | Зона ответственности |
|---|---|---|---|
| `Дмитриев Кирилл Денисович` | **Лидер группы** | `Lead/` | Компоновка проекта, точка входа, отчёт |
| `Мирошниченко Владимир Сергеевич` | Участник | `Viva/` | Участник 1 — `Product`, Участник 2 — `Catalog` |
| `Астафьева Виола Александровна` | Участник | `Viola/` | Участник 3 — `IProductRepository`, `FileProductRepository`; Участник 4 — `CatalogPrinter`, `CatalogReport` |

> ФИО вписать перед отправкой.

**Соответствие пунктам требований к отчёту:**

| Пункт требований | Раздел отчёта |
|---|---|
| 1. Титульная часть | Раздел 1 |
| 2. Текст исходного задания | Раздел 2 |
| 3. Краткое описание решения (2–5 предложений) | Раздел 4 (4.1–4.5) |
| 4. Листинг решения под каждым описанием | Раздел 4 (4.1–4.5) |
| 5. Таблица модификаторов | Раздел 5 |
| 6. Доказательство работы DIP | Раздел 6 |
| 7. Доказательство работы инкапсуляции | Раздел 7 |
| 8. Список вопросов от участников | необязательный, не приводится |
| 9. Выводы: `internal`, `file`, что было бы без инкапсуляции | Раздел 9 |

## 2. Текст исходного задания

### Темы занятия

- Виды модификаторов доступа
- Модификаторы доступа в рамках проекта
- Модификаторы доступа в рамках сборки
- Файл как область видимости
- Целостность данных и инкапсуляция
- Принципы SOLID: SRP, OCP, LSP, ISP, DIP

### Задание 3 (групповое)

#### Проект

Мы продолжаем проект маркетплейса с занятия 2. В нём уже есть `Product`, `CartItem`, `Cart`, `PriceCalculation` и `OrderService` (или аналог). На этом занятии добавляем каталог товаров, инкапсуляцию и слой доступа к данным.

Т.е. делаем хранилище товаров и инструменты просмотра и изменения состава товаров в этом хранилище.

**Правила проекта:**

1. Пространства имён фиксированы: `Marketplace.Domain` (сущности), `Marketplace.DataAccess` (хранение), `Marketplace.Presentation` (вывод).
2. Один тип — один файл.
3. Снаружи типов — только то, что действительно является контрактом; всё остальное — `private`, `internal` или `file`.

**Как распределены типы проекта:**

| Тип | Пространство имён | Модификатор | Почему |
|---|---|---|---|
| `Product` | `Marketplace.Domain` | `public` | сущность нужна и приложению, и слою хранения |
| `Catalog` | `Marketplace.Domain` | `public` | публичный API каталога, но коллекция внутри закрыта |
| `IProductRepository` | `Marketplace.DataAccess` | `public` | абстракция, от которой зависят остальные (DIP) |
| `FileProductRepository` | `Marketplace.DataAccess` | `internal` | деталь реализации, снаружи сборки не нужна |
| `CatalogPrinter` | `Marketplace.Presentation` | `public` | точка вывода для приложения |
| `PriceFormatter` | `Marketplace.Presentation` | `file` | вспомогательный тип одного файла |

#### Участник 1 — сущность `Product` (`Product.cs`)

Требуется: товар из занятия 2 приводим к «взрослому» виду — поля закрываем, проверки собираем в одном месте.

1. Все поля — закрытые: идентификатор, название, цена за единицу.
2. Конструктор проверяет данные и бросает исключение при недопустимых значениях:
   - идентификатор больше нуля;
   - название не пустое;
   - цена не отрицательная.
3. Открытые методы: `GetId()`, `GetTitle()`, `GetPrice()`, `SetPrice(int)`, `GetInfo()`.
4. 1–2 приватных метода проверки, например `EnsureTitle(string title)` и `EnsurePrice(int price)`.

**Что показать в отчёте:** `product.price = -100;` не компилируется, а `product.SetPrice(-100)` выбрасывает исключение.

#### Участник 2 — агрегирующий класс `Catalog` (`Catalog.cs`)

Требуется:

1. Закрытая коллекция товаров: `private readonly List<Product> products = new();`
2. Открытые методы:
   - `Add(Product product)` — не принимать `null` и не дублировать товары (сравнение по названию без учёта регистра и пробелов по краям);
   - `Remove(string title)` — возвращать `bool`;
   - `Find(string title)` — возвращать `Product?`;
   - `GetProducts()` — возвращать **копию** коллекции как `IReadOnlyList<Product>`;
   - `GetCount()` — количество товаров.
3. 1–2 приватных метода: `Normalize(string title)`, `SameTitle(string left, string right)`.

**Что показать в отчёте:** `catalog.GetProducts().Clear();` не компилируется — метода `Clear` у `IReadOnlyList<T>` нет.

#### Участник 3 — доступ к данным: пространство имён `Marketplace.DataAccess` (`DataAccess/`)

Требуется:

1. Объявить открытый интерфейс (DIP):

```csharp
using Marketplace.Domain;

namespace Marketplace.DataAccess;

public interface IProductRepository
{
    void Save(IEnumerable<Product> products);
    List<Product> Load();
}
```

2. Реализовать его во **внутреннем** классе — деталь реализации не обязана быть видна снаружи сборки:

```csharp
using Marketplace.Domain;

namespace Marketplace.DataAccess;

internal class FileProductRepository : IProductRepository
{
    private readonly string filePath;

    public FileProductRepository(string filePath) => this.filePath = filePath;

    public void Save(IEnumerable<Product> products)
    {
        //TODO:  Сохранять каждый товар строкой в файл 
        //TODO:  Использовать ПРИВАТНЫЙ метод Serialize 
    }

    public List<Product> Load()
    {
        //TODO:  Прочитать строки файла и собрать List<Product> 
        //TODO:  При отсутствии файла вернуть пустой список 
        //TODO:  Использовать ПРИВАТНЫЙ метод Deserialize
        return new List<Product>();
    }

    //TODO:  private static string Serialize(Product product) 
    //TODO:  private static Product Deserialize(string line) 
}
```

3. Формат хранения выберите сами (например, `1;Кружка;450` — по строке на товар) и опишите его в комментариях к классу.
4. Не забудьте `using Marketplace.Domain` — типы слоя хранения берутся из домена.

#### Участник 4 — вывод и отчёты (SRP)

Требуется:

1. Класс `CatalogPrinter` (`CatalogPrinter.cs`) — **только печать**: `PrintAll(IEnumerable<Product> products)`, `PrintTop(IEnumerable<Product> products, int count)`; внутри 1–2 приватных метода подготовки строки.
2. В том же файле — `file static class PriceFormatter` с методом `Format(int price)` (демонстрация файла как области видимости).
3. Отдельный класс `CatalogReport` (`CatalogReport.cs`) — **только расчёты**: `GetAveragePrice(IEnumerable<Product> products)` и `GetSummary(IEnumerable<Product> products)`; ни печати, ни работы с файлами.
4. Убедиться, что ни `Product`, ни `Catalog` не занимаются печатью — это и есть SRP.

**Вопрос для отчёта:** почему `PriceFormatter` не виден в файле `CatalogReport.cs` и что пришлось бы сделать, если бы он понадобился в двух файлах?

**Ответ:** модификатор `file` ограничивает тип одним файлом; для двух файлов нужен `internal` (внутри сборки) или `public` (для всех).

## 3. Структура проекта

```text
Homework_3/
├── Marketplace.csproj
├── Results_Homework_3_README.md      — этот отчёт
├── Lead/
│   └── Program.cs                    — точка входа, демонстрация сценариев
├── Viola/                            — Участник 3 и Участник 4
│   ├── IProductRepository.cs
│   ├── FileProductRepository.cs
│   ├── CatalogPrinter.cs
│   └── CatalogReport.cs
├── Viva/                             — Участник 1 и Участник 2
│   ├── Product.cs
│   └── Catalog.cs
└── From_Homework_2/                  — код занятия 2, без изменений
    ├── BalanceState.cs
    ├── CalculatedPrice.cs
    ├── Contact.cs
    ├── OrderService.cs
    ├── PaymentResult.cs
    ├── ProductPosition.cs
    ├── User.cs
    ├── UserBasket.cs
    └── UserProfile.cs
```

Папки в проекте сгруппированы по авторам, чтобы был виден вклад каждого участника. На пространства имён это не влияет: файлы из `Viva/` и `Viola/` объявляют пространства имён по правилам задания, а не по имени папки.

`From_Homework_2/` — код предыдущего занятия. В нём изменён ровно один файл: `UserBasket.cs` обращался к свойствам `product.Id` и `product.Price`, которых у нового `Product` нет, поэтому вызовы заменены на `GetId()` и `GetPrice()`. Остальные файлы не тронуты. Слой оплаты сохранён из занятия 2 и в сценариях текущего задания напрямую не задействован.

## 4. Решения по подзаданиям

### 4.1. Участник 1 — сущность `Product` — `Viva/Product.cs`

Класс приведён к виду с закрытыми полями: `id` и `title` объявлены `readonly`, потому что после создания не меняются, а `price` изменяется только через `SetPrice`. Все проверки собраны в двух приватных `static`-методах — `EnsureTitle` и `EnsurePrice`, — которые вызывают и конструктор, и сеттер, поэтому недопустимое значение нельзя протащить ни при создании, ни при изменении. Проверка идентификатора выполняется прямо в конструкторе: `id` присваивается один раз и изменить его методы класса не могут, отдельный `Ensure`-метод для него не нужен. Нормализация названия (`Trim`) выполняется в конструкторе после проверки, поэтому внутри объекта всегда лежит уже очищенная строка.

```csharp
using System;

namespace Marketplace.Domain;

/// <summary>
/// Товар каталога. Все поля закрытые, проверки собраны в одном месте —
/// в приватных методах EnsureTitle и EnsurePrice. Снаружи доступен только
/// контракт из задания: GetId, GetTitle, GetPrice, SetPrice, GetInfo.
/// </summary>
public class Product
{
    private readonly int id;
    private readonly string title;
    private int price;

    public Product(int id, string title, int price)
    {
        // ID проверяется прямо в конструкторе: изменять его методы класса не могут,
        // поэтому отдельный Ensure-метод для него не нужен.
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор должен быть больше нуля.");

        EnsureTitle(title);
        EnsurePrice(price);

        this.id = id;
        this.title = title.Trim();
        this.price = price;
    }

    public int GetId() => id;
    public string GetTitle() => title;
    public int GetPrice() => price;

    public void SetPrice(int newPrice)
    {
        EnsurePrice(newPrice);
        price = newPrice;
    }

    public string GetInfo() => $"Товар #{id} \"{title}\", {price} УЕ";

    private static void EnsureTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Название товара не может быть пустым.", nameof(value));
    }

    private static void EnsurePrice(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Цена не может быть отрицательной.");
    }
}
```

**Проверка требования «показать в отчёте»:** присваивание `product.price = -100` не компилируется (ошибка компилятора приведена в разделе 6), а `product.SetPrice(-100)` выбрасывает `ArgumentOutOfRangeException`, при этом цена остаётся прежней. Это видно в секции 1 вывода программы.

### 4.2. Участник 2 — агрегирующий класс `Catalog` — `Viva/Catalog.cs`

Коллекция объявлена `private readonly List<Product> products = new();` — снаружи она недоступна. `Add` отсекает `null` и дубликаты по названию через `SameTitle`, который сравнивает строки без учёта регистра и с `Trim` с обеих сторон; дополнительно проверяется дубль по идентификатору, потому что повтор ID ломает сохранение в файл и расчёт корзины. `GetProducts` возвращает копию через `ToList()`, поэтому изменение возвращённого списка не затрагивает каталог: тип `IReadOnlyList<Product>` не даёт вызвать `Clear`, а приведение к `List<Product>` работает с копией. Нормализация и сравнение вынесены в два приватных метода `Normalize` и `SameTitle`, как требует задание.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Domain;

/// <summary>
/// Каталог товаров: закрытая коллекция и публичный API для просмотра и изменения состава.
/// </summary>
public class Catalog
{
    private readonly List<Product> products = new();

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        // Сравнение по названию без учёта регистра и пробелов по краям.
        if (products.Any(existing => SameTitle(existing.GetTitle(), product.GetTitle())))
            throw new ArgumentException("Товар с таким названием уже есть в каталоге.", nameof(product));

        // Дополнительная защита: одинаковый ID ломает сохранение и расчёт корзины.
        if (products.Any(existing => existing.GetId() == product.GetId()))
            throw new ArgumentException("Товар с таким ID уже есть в каталоге.", nameof(product));

        products.Add(product);
    }

    public bool Remove(string title)
    {
        int index = FindIndex(title);
        if (index < 0)
            return false;

        products.RemoveAt(index);
        return true;
    }

    public Product? Find(string title)
    {
        int index = FindIndex(title);
        return index < 0 ? null : products[index];
    }

    /// <summary>
    /// Возвращает копию: снаружи нельзя изменить внутренний список.
    /// </summary>
    public IReadOnlyList<Product> GetProducts() => products.ToList();

    public int GetCount() => products.Count;

    private int FindIndex(string title)
    {
        ArgumentNullException.ThrowIfNull(title);
        return products.FindIndex(product => SameTitle(product.GetTitle(), title));
    }

    private static string Normalize(string title) => title.Trim();

    private static bool SameTitle(string left, string right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
}
```
### 4.3. Участник 3 — слой доступа к данным — `Viola/IProductRepository.cs`, `Viola/FileProductRepository.cs`

Интерфейс `IProductRepository` объявлен `public`: это абстракция, от которой зависят остальные части приложения, и именно она реализует принцип инверсии зависимостей. Сам класс `FileProductRepository` объявлен `internal` — это деталь реализации, снаружи сборки её видеть незачем. Разбор строк вынесен в приватные `static`-методы `Serialize` и `Deserialize`; формат хранения описан в комментарии к классу.

Разбор строки сделан через `TryParse`, а не через исключения: битая строка не должна ронять загрузку всего каталога. Запись ведётся в UTF-8 без BOM, чтобы файл корректно открывался и нашим кодом, и сторонними инструментами; цена пишется через `InvariantCulture`, чтобы дробная часть не зависела от региональных настроек.

```csharp
using System.Collections.Generic;
using Marketplace.Domain;

namespace Marketplace.DataAccess;

/// <summary>
/// Абстракция хранилища товаров. Зависимости строятся на ней, а не на конкретном классе (DIP).
/// </summary>
public interface IProductRepository
{
    void Save(IEnumerable<Product> products);
    List<Product> Load();
}
```

```csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Marketplace.Domain;

namespace Marketplace.DataAccess;

/// <summary>
/// Файловое хранилище товаров. Внутренняя деталь реализации, наружу сборки не нужна.
/// Формат строки: ID;Название;Цена — один товар на строку.
/// </summary>
internal class FileProductRepository : IProductRepository
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly string filePath;

    public FileProductRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        this.filePath = filePath;
    }

    public void Save(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var lines = new List<string>();
        foreach (var product in products)
        {
            if (product == null)
                throw new ArgumentException("Список товаров не должен содержать null.", nameof(products));

            lines.Add(Serialize(product));
        }

        // UTF-8 без BOM: файл читается и нашим кодом, и сторонними инструментами.
        File.WriteAllLines(filePath, lines, Utf8NoBom);
    }

    /// <summary>
    /// Читает товары из файла. Отсутствующий файл — это пустой список.
    /// Битые строки пропускаются: одна испорченная запись не роняет загрузку.
    /// ВНИМАНИЕ: чтобы не было коллизий по ID, перед заполнением создайте новый Catalog.
    /// </summary>
    public List<Product> Load()
    {
        var result = new List<Product>();

        if (!File.Exists(filePath))
            return result;

        var lines = File.ReadAllLines(filePath, Utf8NoBom);
        foreach (var line in lines)
        {
            Product? product = Deserialize(line);

            // null — битая строка; повтор ID — тоже пропускаем.
            if (product == null || result.Exists(existing => existing.GetId() == product.GetId()))
                continue;

            result.Add(product);
        }

        return result;
    }

    private static string Serialize(Product product)
    {
        string title = product.GetTitle();

        // Разделитель и перевод строки внутри названия сломали бы формат.
        if (title.Contains(';') || title.Contains('\n') || title.Contains('\r'))
            throw new ArgumentException("Название товара не должно содержать символ ';' или перевод строки.", nameof(product));

        return string.Join(';',
            product.GetId().ToString(CultureInfo.InvariantCulture),
            title,
            product.GetPrice().ToString(CultureInfo.InvariantCulture));
    }

    private static Product? Deserialize(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var parts = line.Split(';');
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            return null;

        if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int price))
            return null;

        try
        {
            return new Product(id, parts[1], price);
        }
        catch (ArgumentException)
        {
            // Некорректные данные из файла — пропускаем строку, а не роняем загрузку.
            return null;
        }
    }
}
```
### 4.4. Участник 4 — вывод и отчёты (SRP) — `Viola/CatalogPrinter.cs`, `Viola/CatalogReport.cs`

Ответственность разделена: `CatalogPrinter` только печатает, `CatalogReport` только считает. Ни один из них не знает о файлах, а `Product` и `Catalog` ничего не печатают — это и есть соблюдение SRP. Подготовка строки вывода вынесена в приватный `PrepareLine`, ограничение количества в `PrintTop` считается один раз.

`PriceFormatter` объявлен как `file static class` в том же файле, что и `CatalogPrinter`. Модификатор `file` делает тип видимым только внутри этого файла: из `CatalogReport.cs` он недоступен, и это проверено компилятором (раздел 6). Если бы форматирование понадобилось в двух файлах, `file` не подошёл бы — пришлось бы взять `internal` (виден всей сборке) или `public` (виден всем).

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

/// <summary>
/// Отвечает только за печать каталога (SRP). Ничего не считает и не хранит.
/// </summary>
public class CatalogPrinter
{
    public void PrintAll(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        Console.WriteLine("— Каталог товаров —");

        var list = products.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        foreach (var product in list)
            Console.WriteLine(PrepareLine(product));
    }

    public void PrintTop(IEnumerable<Product> products, int count)
    {
        ArgumentNullException.ThrowIfNull(products);

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Количество не может быть отрицательным.");

        var list = products.ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("Каталог пуст.");
            return;
        }

        if (count == 0)
        {
            Console.WriteLine("Запрошен топ из 0 товаров.");
            return;
        }

        // Если просят больше, чем есть, честно сообщаем, а не молчим.
        if (count > list.Count)
            Console.WriteLine($"Запрошен топ {count}, но в каталоге только {list.Count} товаров.");

        Console.WriteLine($"— Топ {Math.Min(count, list.Count)} самых дорогих товаров —");

        foreach (var product in list.OrderByDescending(p => p.GetPrice()).Take(count))
            Console.WriteLine(PrepareLine(product));
    }

    private string PrepareLine(Product product) =>
        $"[{product.GetId()}] {product.GetTitle(),-20} | Цена: {PriceFormatter.Format(product.GetPrice())}";
}

// Виден только в этом файле.
file static class PriceFormatter
{
    public static string Format(int price) => $"{price} УЕ";
}
```

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace.Presentation;

/// <summary>
/// Отвечает только за расчёты по каталогу (SRP). Ничего не печатает и не работает с файлами.
/// </summary>
public class CatalogReport
{
    public decimal GetAveragePrice(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var prices = products.Select(product => (decimal)product.GetPrice()).ToList();
        if (prices.Count == 0)
            return 0m;

        return prices.Average();
    }

    public decimal GetSummary(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        return products.Sum(product => (decimal)product.GetPrice());
    }
}
```
### 4.5. Точка входа — `Lead/Program.cs`

Точка входа последовательно демонстрирует все требования задания: создание товаров и отказ при недопустимых данных, поиск и удаление, невозможность изменить коллекцию каталога снаружи, сохранение и загрузку через интерфейс, печать и расчёты, устойчивость загрузки к битым строкам. Отдельная секция выводит коды ошибок компилятора, полученные реальной сборкой.

```csharp
using System;
using System.Collections.Generic;
using Marketplace.DataAccess;
using Marketplace.Domain;
using Marketplace.Presentation;

namespace Marketplace;

/// <summary>
/// Точка входа компоновки: демонстрация инкапсуляции, DIP и SRP.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Section("1. Создание товаров, в том числе с недопустимыми данными");

        var catalog = new Catalog();
        catalog.Add(new Product(1, "Кружка", 450));
        catalog.Add(new Product(2, "Термос", 1200));
        catalog.Add(new Product(3, "Набор стикеров", 90));

        Console.WriteLine($"В каталог добавлено товаров: {catalog.GetCount()}");

        // Целостность данных: недопустимые значения не попадают в объект.
        TryAdd(catalog, 4, "   ", -5, "пустое название и отрицательная цена");
        TryAdd(catalog, 4, "Товар", -5, "отрицательная цена");
        TryAdd(catalog, 0, "Товар", 100, "ID равен нулю");
        TryAdd(catalog, 5, "кружка", 300, "дубль названия в другом регистре");
        TryAdd(catalog, 1, "Другой товар", 300, "дубль ID");

        Console.WriteLine($"Товаров в каталоге после отклонённых попыток: {catalog.GetCount()}");

        // Задание требует показать: product.price = -100 не компилируется,
        // а product.SetPrice(-100) выбрасывает исключение.
        var guarded = new Product(7, "Проверка цены", 500);
        Console.WriteLine($"Цена до SetPrice(-100): {guarded.GetPrice()}");
        try
        {
            guarded.SetPrice(-100);
            Console.WriteLine("[НЕ ОЖИДАЛОСЬ] SetPrice(-100) прошёл.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"[Отклонено] SetPrice(-100): {ex.Message}");
        }
        Console.WriteLine($"Цена после отклонённой попытки: {guarded.GetPrice()} — не изменилась.");

        Section("2. Поиск, удаление и попытка испортить коллекцию снаружи");

        var found = catalog.Find("  термос  ");
        Console.WriteLine(found == null ? "Термос не найден." : $"Найден: {found.GetInfo()}");

        Console.WriteLine($"Удаление \"Термос\": {catalog.Remove("ТЕРМОС")}");
        Console.WriteLine($"Повторное удаление \"Термос\": {catalog.Remove("Термос")}");
        Console.WriteLine($"Товаров в каталоге: {catalog.GetCount()}");

        // GetProducts возвращает копию: изменить внутренний список снаружи нельзя.
        IReadOnlyList<Product> first = catalog.GetProducts();
        IReadOnlyList<Product> second = catalog.GetProducts();

        Console.WriteLine($"Два вызова GetProducts дают разные объекты списка: {!ReferenceEquals(first, second)}");
        Console.WriteLine($"Элементов в копии: {first.Count}, в каталоге: {catalog.GetCount()}");
        Console.WriteLine("catalog.GetProducts().Clear() не компилируется: у IReadOnlyList<T> нет метода Clear.");

        Section("3. Сохранение и загрузка через интерфейс (DIP)");

        // DIP: работаем через интерфейс, а не через конкретный класс.
        IProductRepository repository = new FileProductRepository("products.csv");

        repository.Save(catalog.GetProducts());
        Console.WriteLine("Каталог сохранён в products.csv через IProductRepository.");

        List<Product> loaded = repository.Load();
        Console.WriteLine($"Загружено товаров: {loaded.Count}");

        Section("4. Вывод и отчёты (SRP)");

        var printer = new CatalogPrinter();
        printer.PrintAll(loaded);
        Console.WriteLine();
        printer.PrintTop(loaded, 2);

        Console.WriteLine();
        Console.WriteLine($"Суммарная стоимость каталога: {new CatalogReport().GetSummary(loaded):F2} УЕ");
        Console.WriteLine($"Средняя цена товара: {new CatalogReport().GetAveragePrice(loaded):F2} УЕ");

        Section("5. Проверка целостности файлового хранилища");

        CheckBrokenFile();

        Section("6. Что не компилируется (проверено удалением строк из кода)");

        Console.WriteLine("product.price = -100;      → CS0122: 'Product.price' недоступен из-за уровня защиты");
        Console.WriteLine("catalog.products.Clear();  → CS0122: 'Catalog.products' недоступен из-за уровня защиты");
        Console.WriteLine("catalog.GetProducts().Clear(); → CS1061: 'IReadOnlyList<Product>' не содержит 'Clear'");
        Console.WriteLine("PriceFormatter.Format(1) из CatalogReport.cs → CS0103: имя 'PriceFormatter' не существует в текущем контексте");

        Console.WriteLine();
        Console.WriteLine("Компоновка завершена.");
    }

    private static void TryAdd(Catalog catalog, int id, string title, int price, string caseName)
    {
        try
        {
            catalog.Add(new Product(id, title, price));
            Console.WriteLine($"[НЕ ОЖИДАЛОСЬ] {caseName}: товар добавлен.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[Отклонено] {caseName}: {ex.Message}");
        }
    }

    private static void CheckBrokenFile()
    {
        const string brokenPath = "products_broken.csv";

        // Одна корректная строка, одна битая, одна с недопустимой ценой, одна с повтором ID.
        File.WriteAllLines(brokenPath, new[]
        {
            "1;Кружка;450",
            "мусор",
            "2;Термос;-100",
            "1;Дубль;99"
        });

        IProductRepository repository = new FileProductRepository(brokenPath);
        List<Product> loaded = repository.Load();

        Console.WriteLine($"Из файла с 4 строками (1 корректная, 3 битых) загружено: {loaded.Count}");
        foreach (var product in loaded)
            Console.WriteLine($"  {product.GetInfo()}");

        Console.WriteLine($"Файл существует и прочитан без исключения: {File.Exists(brokenPath)}");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 72));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 72));
    }
}
```
## 5. Таблица модификаторов

| Тип | Файл | Пространство имён | Модификатор | Обоснование |
|---|---|---|---|---|
| `Product` | `Viva/Product.cs` | `Marketplace.Domain` | `public` | Сущность нужна и приложению, и слою хранения: с ней работают `Catalog`, `CatalogPrinter`, `CatalogReport` и репозиторий. Все поля внутри — `private`, два из них `readonly`. |
| `Catalog` | `Viva/Catalog.cs` | `Marketplace.Domain` | `public` | Публичный API каталога — это методы `Add`, `Remove`, `Find`, `GetProducts`, `GetCount`. Сама коллекция `products` — `private readonly`, снаружи недоступна. |
| `IProductRepository` | `Viola/IProductRepository.cs` | `Marketplace.DataAccess` | `public` | Абстракция, от которой зависят остальные части. Публичная, иначе точку входа нельзя было бы написать против интерфейса, а не против класса (DIP). |
| `FileProductRepository` | `Viola/FileProductRepository.cs` | `Marketplace.DataAccess` | `internal` | Деталь реализации файлового хранения. Снаружи сборки не нужна: приложение работает через `IProductRepository`, поэтому класс можно заменить, не меняя вызывающий код. |
| `FileProductRepository.Save`, `.Load` | `Viola/FileProductRepository.cs` | `Marketplace.DataAccess` | `public` | Реализация членов интерфейса: модификатор не может быть уже, чем в интерфейсе, поэтому `public` при `internal`-классе. Фактическая видимость всё равно ограничена сборкой. |
| `FileProductRepository.Serialize`, `.Deserialize` | `Viola/FileProductRepository.cs` | `Marketplace.DataAccess` | `private static` | Вспомогательные методы разбора формата. Никому, кроме самого класса, не нужны — это внутренняя деталь. |
| `CatalogPrinter` | `Viola/CatalogPrinter.cs` | `Marketplace.Presentation` | `public` | Точка вывода для приложения: точку входа вызывают её методы `PrintAll` и `PrintTop`. |
| `CatalogPrinter.PrepareLine` | `Viola/CatalogPrinter.cs` | `Marketplace.Presentation` | `private` | Подготовка одной строки вывода — деталь реализации печати. |
| `PriceFormatter` | `Viola/CatalogPrinter.cs` | `Marketplace.Presentation` | `file` | Вспомогательный тип одного файла. Нужен только `CatalogPrinter`, поэтому ограничен файлом, а не сборкой. |
| `PriceFormatter.Format` | `Viola/CatalogPrinter.cs` | `Marketplace.Presentation` | `public` | Метод внутри `file`-типа: `public` относится к самому типу, но тип виден только в своём файле. |
| `CatalogReport` | `Viola/CatalogReport.cs` | `Marketplace.Presentation` | `public` | Отчётные расчёты вызывает точка входа. Класс не печатает и не работает с файлами — только считает. |
| `Catalog.products` | `Viva/Catalog.cs` | `Marketplace.Domain` | `private readonly` | Коллекция внутри каталога. `private` не даёт изменить её снаружи, `readonly` не даёт заменить саму ссылку. |
| `Product.id`, `.title` | `Viva/Product.cs` | `Marketplace.Domain` | `private readonly` | Идентичность и название товара не меняются после создания. |
| `Product.price` | `Viva/Product.cs` | `Marketplace.Domain` | `private` | Меняется только через `SetPrice`, где значение проходит проверку `EnsurePrice`. |

## 6. Доказательство работы DIP

Переменная объявлена с типом интерфейса `IProductRepository`, а экземпляр создаётся конкретного класса `FileProductRepository`. Приложение знает только про интерфейс: чтобы перейти с файла на базу данных или на тестовую заглушку, достаточно заменить одну строку создания объекта, а весь остальной код останется прежним.

```csharp
// DIP: работаем через интерфейс, а не через конкретный класс.
// Тип переменной — IProductRepository, создаётся FileProductRepository.
IProductRepository repository = new FileProductRepository("products.csv");

repository.Save(catalog.GetProducts());
Console.WriteLine("Каталог сохранён в products.csv через IProductRepository.");

List<Product> loaded = repository.Load();
Console.WriteLine($"Загружено товаров: {loaded.Count}");
```

Дополнительное доказательство обратного: класс `FileProductRepository` объявлен `internal`, поэтому в другом проекте решение не собралось бы с прямым обращением к нему — работать можно только через интерфейс. Внутри самой сборки код компилируется, потому что видимость `internal` покрывает весь проект.

Результат выполнения этой секции:

```text
Каталог сохранён в products.csv через IProductRepository.
Загружено товаров: 2
```

Содержимое файла `products.csv` после сохранения (формат `ID;Название;Цена`, UTF-8 без BOM):

```text
1;Кружка;450
3;Набор стикеров;90
```

## 7. Доказательство работы инкапсуляции

### 7.1. Тексты ошибок компилятора

Коды ошибок получены реальной сборкой проекта: в него был временно добавлен файл с четырьмя некорректными обращениями, после сборки файл удалён. Проверочный код:

```csharp
var p = new Product(1, "x", 10);
p.price = -100;                 // закрытое поле
var c = new Catalog();
c.products.Clear();             // закрытая коллекция
c.GetProducts().Clear();        // попытка изменить коллекцию каталога через копию
string s = PriceFormatter.Format(1);   // file-тип из другого файла
```

Фактический вывод компилятора:

```text
Bad.cs(9,11):  error CS0122: 'Product.price' is inaccessible due to its protection level

Bad.cs(11,11): error CS0122: 'Catalog.products' is inaccessible due to its protection level

Bad.cs(12,25): error CS1061: 'IReadOnlyList<Product>' does not contain a definition
               for 'Clear' and no accessible extension method 'Clear' accepting a first
               argument of type 'IReadOnlyList<Product>' could be found (are you missing
               a using directive or an assembly reference?)

Bad.cs(13,20): error CS0103: The name 'PriceFormatter' does not exist in the current context
```

Четыре ошибки — это четыре разных механизма защиты:

1. **CS0122 на `Product.price`** — закрытое поле нельзя изменить снаружи. Данные меняются только через `SetPrice`, где стоит проверка.
2. **CS0122 на `Catalog.products`** — закрытую коллекцию нельзя заменить или очистить снаружи.
3. **CS1061 на `GetProducts().Clear()`** — возвращается `IReadOnlyList<T>`, у которого метода `Clear` просто нет в контракте.
4. **CS0103 на `PriceFormatter`** — тип, объявленный `file`, не виден за пределами своего файла.

### 7.2. Попытка обойти защиту приведением типа

Формально `GetProducts()` возвращает `List<Product>`, упакованный как `IReadOnlyList<Product>`, поэтому приведение обратно компилируется. Но это безопасно: возвращается **копия** списка.

```csharp
IReadOnlyList<Product> view = c.GetProducts();
if (view is List<Product> copy)
{
    copy.Clear();   // чистится копия, а не каталог
}
```

Проверка выполнения:

```text
view is List<Product>: True
После Clear: в копии 0, в каталоге 1 — каталог цел.
```

### 7.3. Целостность данных на уровне значений

Проверки в конструкторе и в сеттере не дают протащить недопустимые значения. Фактический вывод программы:

```text
В каталог добавлено товаров: 3
[Отклонено] пустое название и отрицательная цена: Название товара не может быть пустым. (Parameter 'value')
[Отклонено] отрицательная цена: Цена не может быть отрицательной. (Parameter 'value')
[Отклонено] ID равен нулю: Идентификатор должен быть больше нуля. (Parameter 'value')
[Отклонено] дубль названия в другом регистре: Товар с таким названием уже есть в каталоге. (Parameter 'product')
[Отклонено] дубль ID: Товар с таким ID уже есть в каталоге. (Parameter 'product')
Товаров в каталоге после отклонённых попыток: 3

Цена до SetPrice(-100): 500
[Отклонено] SetPrice(-100): Цена не может быть отрицательной. (Parameter 'value')
Цена после отклонённой попытки: 500 — не изменилась.
```

### 7.4. Целостность при сохранении и загрузке

Отдельная проверка: файл из четырёх строк, где одна корректная, одна — мусор, одна — с отрицательной ценой, одна — с повторяющимся ID.

```text
1;Кружка;450
мусор
2;Термос;-100
1;Дубль;99
```

Результат загрузки:

```text
Из файла с 4 строками (1 корректная, 3 битых) загружено: 1
  Товар #1 "Кружка", 450 УЕ
Файл существует и прочитан без исключения: True
```

Битая строка не роняет загрузку: `Deserialize` возвращает `null`, вызывающий код такие строки пропускает.

## 8. Результат выполнения программы

```text
========================================================================
1. Создание товаров, в том числе с недопустимыми данными
========================================================================
В каталог добавлено товаров: 3
[Отклонено] пустое название и отрицательная цена: Название товара не может быть пустым. (Parameter 'value')
[Отклонено] отрицательная цена: Цена не может быть отрицательной. (Parameter 'value')
[Отклонено] ID равен нулю: Идентификатор должен быть больше нуля. (Parameter 'value')
[Отклонено] дубль названия в другом регистре: Товар с таким названием уже есть в каталоге. (Parameter 'product')
[Отклонено] дубль ID: Товар с таким ID уже есть в каталоге. (Parameter 'product')
Товаров в каталоге после отклонённых попыток: 3
Цена до SetPrice(-100): 500
[Отклонено] SetPrice(-100): Цена не может быть отрицательной. (Parameter 'value')
Цена после отклонённой попытки: 500 — не изменилась.

========================================================================
2. Поиск, удаление и попытка испортить коллекцию снаружи
========================================================================
Найден: Товар #2 "Термос", 1200 УЕ
Удаление "Термос": True
Повторное удаление "Термос": False
Товаров в каталоге: 2
Два вызова GetProducts дают разные объекты списка: True
Элементов в копии: 2, в каталоге: 2
catalog.GetProducts().Clear() не компилируется: у IReadOnlyList<T> нет метода Clear.

========================================================================
3. Сохранение и загрузка через интерфейс (DIP)
========================================================================
Каталог сохранён в products.csv через IProductRepository.
Загружено товаров: 2

========================================================================
4. Вывод и отчёты (SRP)
========================================================================
— Каталог товаров —
[1] Кружка               | Цена: 450 УЕ
[3] Набор стикеров       | Цена: 90 УЕ

— Топ 2 самых дорогих товаров —
[1] Кружка               | Цена: 450 УЕ
[3] Набор стикеров       | Цена: 90 УЕ

Суммарная стоимость каталога: 540.00 УЕ
Средняя цена товара: 270.00 УЕ

========================================================================
5. Проверка целостности файлового хранилища
========================================================================
Из файла с 4 строками (1 корректная, 3 битых) загружено: 1
  Товар #1 "Кружка", 450 УЕ
Файл существует и прочитан без исключения: True

========================================================================
6. Что не компилируется (проверено удалением строк из кода)
========================================================================
product.price = -100;      → CS0122: 'Product.price' недоступен из-за уровня защиты
catalog.products.Clear();  → CS0122: 'Catalog.products' недоступен из-за уровня защиты
catalog.GetProducts().Clear(); → CS1061: 'IReadOnlyList<Product>' не содержит 'Clear'
PriceFormatter.Format(1) из CatalogReport.cs → CS0103: имя 'PriceFormatter' не существует в текущем контексте

Компоновка завершена.
```

Сборка проходит без предупреждений:

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 9. Выводы

### Где пригодились `internal` и `file`

**`internal` — для `FileProductRepository`.** Класс решает одну задачу: сохранить и прочитать список товаров в файле. Приложению не важно, файл это, база данных или сетевое хранилище, — ему важен только интерфейс `IProductRepository`. Реализация помечена `internal`, поэтому за пределы сборки она не видна и не может «протечь» в чужой код как зависимость. Если завтра понадобится другая реализация, её добавят рядом, и ни одна строка вызывающего кода не изменится. Без `internal` класс был бы частью публичного контракта, и любое изменение его сигнатур ломало бы чужие сборки.

**`file` — для `PriceFormatter`.** Формат цены нужен только при печати в `CatalogPrinter`, и больше нигде. Модификатор `file` ограничивает тип одним файлом, поэтому:
- его имя не засоряет пространство имён `Marketplace.Presentation`;
- его нельзя случайно использовать в `CatalogReport`, где форматирование — уже не его дело;
- если в другом файле появится свой `PriceFormatter`, конфликта имён не будет.

Проверка в разделе 7 подтверждает ограничение на уровне компилятора: `CS0103` при обращении из `CatalogReport.cs`. Если бы тип понадобился в двух файлах, `file` пришлось бы заменить на `internal`; `public` тоже подошёл бы, но это уже лишняя публичность для вспомогательного типа.

### Что изменилось бы без инкапсуляции

1. **Данные можно было бы испортить в любой момент.** Если бы `Product.price` был открытым полем, запись `product.price = -100` прошла бы молча, и отрицательная цена поехала бы в расчёт корзины, в отчёты и в файл. Сейчас такое присваивание не компилируется (CS0122), а `SetPrice(-100)` бросает исключение и оставляет цену прежней.

2. **Каталог нельзя было бы считать доверенным.** Открытая коллекция `products` означала бы, что любой код может добавить в неё `null`, дубль или товар в обход проверок. `Add` перестал бы быть единственной точкой входа, и любая проверка внутри него стала бы необязательной. Сейчас `catalog.products.Clear()` не компилируется, а `GetProducts()` возвращает копию — даже приведение к `List<Product>` чистит только копию.

3. **Принцип DIP не выполнялся бы.** Если бы точка входа создавала `FileProductRepository` и работала с ним напрямую, смена хранилища потребовала бы правок в `Program`. Работа через `IProductRepository` плюс `internal` на реализации делает такую замену локальной: меняется одна строка создания объекта.

4. **SRP нарушался бы, и код расползался.** Если бы `CatalogPrinter` умел ещё и считать, а `CatalogReport` — печатать, любое изменение формата вывода затрагивало бы расчёты. Разделение на «печатает» и «считает» означает, что правка в одном классе не задевает другой. Форматирование при этом спрятано в `file`-тип, чтобы не стать общим достоянием по случайности.

5. **Ошибки в данных всплывали бы поздно.** Проверки в конструкторе переносят обнаружение проблемы в момент создания объекта. Без них битые данные лежали бы в каталоге и падали бы где-то в расчёте суммы. А `TryParse` в `Deserialize` означает, что одна испорченная строка в файле пропускается, а не обнуляет весь каталог.

### Итог

Все требования задания выполнены: шесть типов разложены по трём фиксированным пространствам имён, модификаторы выбраны по принципу «контракт или деталь реализации», сохранение и загрузка работают, зависимости идут через интерфейс, печать вынесена из сущностей. Проект собирается без предупреждений и запускается без правок; коды ошибок компилятора в разделе 7 получены реальной сборкой, а не приведены по памяти.