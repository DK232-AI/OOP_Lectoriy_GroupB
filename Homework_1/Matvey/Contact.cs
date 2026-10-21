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
        Id = id;
        CommunicationChannelId = communicationChannelId;
        Identifier = identifier;
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