using System;
using System.Collections.Generic;

public class UserProfile
{
    public User User { get; }
    private readonly List<Contact> _contacts;

    public IReadOnlyList<Contact> Contacts => _contacts;

    public UserProfile(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        _contacts = new List<Contact>();
    }

    // метод добавления контакта
    public void AddContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        if (_contacts.Contains(contact))
            throw new ArgumentException("Этот контакт уже добавлен в профиль.", nameof(contact));

        _contacts.Add(contact);
    }

    // метод удаления контакта
    public bool RemoveContact(int contactId)
    {
        var contactToRemove = _contacts.Find(c => c.Id == contactId);
        if (contactToRemove != null)
            return _contacts.Remove(contactToRemove);
        return false;
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
                var (contactId, channelId, identifier) = contact;
                sb.AppendLine($"Контакт канала {channelId}: {identifier}");
            }
        }
        contactsInfo = sb.ToString();
    }
}
