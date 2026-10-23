public class CommunicationChannel
{
    public int Id { get; }
    public string Name { get; }

    public CommunicationChannel(int id, string name)
    {
        if (id <= 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new System.ArgumentException("Название канала обязательно.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
    }

    public override bool Equals(object? obj)
    {
        return obj is CommunicationChannel other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public void Deconstruct(out int id, out string name)
    {
        id = Id;
        name = Name;
    }
}
