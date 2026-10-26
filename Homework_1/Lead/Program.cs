using System;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== 1. Простая компоновка (без сервиса) ===");

        var email = new CommunicationChannel(1, "Email");
        var phone = new CommunicationChannel(2, "Телефон");

        var user = new User(1, "Иванов Иван Иванович", "iivanov", "hash_abc123");
        var profile = new UserProfile(user);

        profile.AddContact(new Contact(1, email.Id, "ivanov@mail.ru"));
        profile.AddContact(new Contact(2, phone.Id, "+79991234567"));

        var (uid, fullName, info) = profile;
        Console.WriteLine($"Пользователь {uid}: {fullName}");
        Console.Write(info);

        Console.WriteLine("\n=== 2. Работа через сервисный класс ===");

        var service = new UserService();
        service.RegisterChannel(email);
        service.RegisterChannel(phone);

        var p = service.CreateProfile("Петрова Анна Сергеевна", "apetrova", "hash_xyz789");
        Console.WriteLine($"Создан профиль: id={p.User.Id}, логин={p.User.Login}");

        service.AddContact(p.User.Id, email.Id, "petrova@mail.ru");
        var tg = service.AddContact(p.User.Id, phone.Id, "+79990001122");
        Console.WriteLine($"Добавлен контакт id={tg.Id}");

        service.ChangeFullName(p.User.Id, "Петрова Анна Сергеевна (Смирнова)");
        Console.WriteLine("ФИО изменено.");

        var stats = service.GetStats(p.User.Id);
        Console.WriteLine($"Статистика: пользователь {stats.UserId}, '{stats.FullName}', " +
                          $"контактов {stats.ContactsCount}, каналов {stats.ChannelsUsed}");

        Console.WriteLine("Контакты:");
        foreach (var line in service.ListContactsByChannel(p.User.Id))
            Console.WriteLine("  " + line);

        service.RemoveContact(p.User.Id, tg.Id);
        Console.WriteLine($"После удаления контакта {tg.Id}: контактов {service.GetStats(p.User.Id).ContactsCount}");

        Console.WriteLine("\n=== 3. Ошибки, которые сервис должен ловить ===");
        TryRun("дубль логина", () => service.CreateProfile("Другой", "APETROVA", "h"));
        TryRun("неизвестный канал", () => service.AddContact(p.User.Id, 99, "x"));
        TryRun("пустой идентификатор", () => service.AddContact(p.User.Id, email.Id, "  "));
        TryRun("несуществующий пользователь", () => service.GetStats(777));
    }

    private static void TryRun(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"{title}: НЕ поймано");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{title}: {e.GetType().Name} — {e.Message}");
        }
    }
}