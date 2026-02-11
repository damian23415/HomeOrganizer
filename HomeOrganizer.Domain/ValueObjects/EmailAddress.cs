namespace HomeOrganizer.Domain.ValueObjects;

public class EmailAddress : IEquatable<EmailAddress>
{
  public string Value { get; }

  public EmailAddress()
  {
    // EF
  }

  public EmailAddress(string value)
  {
    if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
      throw new ArgumentException("Invalid email address format.");
    
    Value = value.ToLowerInvariant();
  }
  
  public bool Equals(EmailAddress? other) => other?.Value == Value;
  public override bool Equals(object? obj) => Equals(obj as EmailAddress);
  public override int GetHashCode() => Value.GetHashCode();
}