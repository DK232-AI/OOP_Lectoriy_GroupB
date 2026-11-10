using System;

namespace Marketplace.Domain;

/// <summary>
/// Контакт пользователя. Оставлен из Домашнего Задания 1.
/// ВНИМАНИЕ: восстановлен по деконструктору (_, channelId, identifier) в UserProfile.Deconstruct.
/// Заменить на исходный класс из ДЗ 1, если он отличался.
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
