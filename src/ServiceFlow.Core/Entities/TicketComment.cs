namespace ServiceFlow.Core.Entities;

public class TicketComment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TicketId { get; private set; }
    public string Text { get; private set; }
    public string Author { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public TicketComment(Guid ticketId, string text, string author)
    {
        TicketId = ticketId;
        Text = text;
        Author = author;
    }
}