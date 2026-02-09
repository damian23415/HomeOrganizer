using HomeOrganizer.Domain.ValueObjects;

namespace HomeOrganizer.Domain.Entities.Messages;

public class OutgoingMessage : BaseEntity
{
  public EmailAddress RecipientEmail { get; private set; }
  public string Subject { get; private set; }
  public string Body { get; private set; }
  public DateTime? HandledAt { get; private set; }
  
  private OutgoingMessage() {} // EF
  
  public OutgoingMessage(EmailAddress recipientEmail, string subject, string body)
  {
    RecipientEmail = recipientEmail ?? throw new ArgumentNullException(nameof(recipientEmail));
    Subject = subject ?? throw new ArgumentNullException(nameof(subject));
    Body = body ?? throw new ArgumentNullException(nameof(body));
  }
  
  public void MarkHandled()
  {
    HandledAt = DateTime.UtcNow;
  }
}