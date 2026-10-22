public class Contact
{
    public int Id { get; set; }

    public int CommunicationChannelId { get; set; }

    public string Identifier { get; set; }

    public Contact(
        int id,
        int communicationChannelId,
        string identifier)
    {
        if (id < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(id));
        }

        if (communicationChannelId <= 0)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(communicationChannelId));
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new System.ArgumentException(
                "Идентификатор обязателен.",
                nameof(identifier));
        }

        Id = id;
        CommunicationChannelId = communicationChannelId;
        Identifier = identifier.Trim();
    }

    public bool IsPersisted => Id > 0;

    public override bool Equals(object? obj)
    {
        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is Contact other
            && IsPersisted
            && other.IsPersisted
            && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return IsPersisted ? Id.GetHashCode() : base.GetHashCode();
    }

    public void Deconstruct(
        out int contactId,
        out int communicationChannelId,
        out string identifier)
    {
        contactId = Id;
        communicationChannelId = CommunicationChannelId;
        identifier = Identifier;
    }
}
