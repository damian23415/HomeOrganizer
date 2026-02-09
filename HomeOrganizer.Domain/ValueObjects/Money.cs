namespace HomeOrganizer.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
  public decimal Value { get; }

  public Money(decimal value)
  {
    if (value < 0)
      throw new ArgumentOutOfRangeException(nameof(value), value, "Value cannot be negative.");

    Value = Math.Round(value, 2);
  }
  
  public bool Equals(Money? other) => other != null && Value == other.Value;
  public override bool Equals(object? obj) => Equals(obj as Money);
  public override int GetHashCode() => Value.GetHashCode();
  
  public Money Add(Money other) => new(Value + other.Value);
  
  // Conversions
  public static implicit operator decimal(Money money) => money.Value;
  public static implicit operator Money(decimal value) => new(value);
}