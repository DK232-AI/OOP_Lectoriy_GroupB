using System;

namespace Marketplace.Domain;

/// <summary>
/// Пользователь платформы. Оставлен из Домашнего Задания 1.
/// ВНИМАНИЕ: восстановлен по использованию в UserProfile и Contact.
/// Заменить на исходный класс из ДЗ 1, если он отличался.
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
