using System;
using System.Collections.Generic;
using System.Linq;

public class UserService
{
    private readonly Dictionary<int, UserProfile> _profiles = new();
    private readonly Dictionary<int, CommunicationChannel> _channels = new();
    private int _nextUserId = 1;
    private int _nextContactId = 1;

    public void RegisterChannel(CommunicationChannel channel)
    {
        if (channel == null) throw new ArgumentNullException(nameof(channel));
        if (_channels.ContainsKey(channel.Id))
            throw new InvalidOperationException($"Канал {channel.Id} уже зарегистрирован.");
        _channels.Add(channel.Id, channel);
    }

    public UserProfile CreateProfile(string fullName, string login, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Логин обязателен.", nameof(login));

        if (_profiles.Values.Any(p => p.User.Login.Equals(login.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Логин '{login.Trim()}' уже занят.");

        var user = new User(_nextUserId, fullName, login, passwordHash);
        _nextUserId++;

        var profile = new UserProfile(user);
        _profiles.Add(user.Id, profile);
        return profile;
    }

    public UserProfile GetProfile(int userId) =>
        _profiles.TryGetValue(userId, out var p)
            ? p
            : throw new KeyNotFoundException($"Профиль пользователя {userId} не найден.");

    public void ChangeFullName(int userId, string newFullName) =>
        GetProfile(userId).User.ChangeFullName(newFullName);

    public Contact AddContact(int userId, int channelId, string identifier)
    {
        if (!_channels.ContainsKey(channelId))
            throw new InvalidOperationException($"Канал {channelId} не зарегистрирован.");

        var profile = GetProfile(userId);
        var contact = new Contact(_nextContactId, channelId, identifier);
        profile.AddContact(contact);
        _nextContactId++;
        return contact;
    }

    public bool RemoveContact(int userId, int contactId) =>
        GetProfile(userId).RemoveContact(contactId);

    public (int UserId, string FullName, int ContactsCount, int ChannelsUsed) GetStats(int userId)
    {
        var profile = GetProfile(userId);
        var (id, fullName, _) = profile;
        int channelsUsed = profile.Contacts
            .Select(c => c.CommunicationChannelId)
            .Distinct()
            .Count();
        return (id, fullName, profile.Contacts.Count, channelsUsed);
    }

    public IEnumerable<string> ListContactsByChannel(int userId)
    {
        foreach (var contact in GetProfile(userId).Contacts)
        {
            var (_, channelId, identifier) = contact;
            var channelName = _channels.TryGetValue(channelId, out var ch)
                ? ch.Name
                : "неизвестный канал";
            yield return $"{channelName}: {identifier}";
        }
    }
}
