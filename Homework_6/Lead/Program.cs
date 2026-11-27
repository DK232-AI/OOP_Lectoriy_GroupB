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
