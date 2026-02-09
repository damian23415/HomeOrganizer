using HomeOrganizer.Domain.Enums;

namespace HomeOrganizer.Domain.ValueObjects;

public class UserRole : IEquatable<UserRole>
{
  public UserRoleEnum Value { get; }

  private UserRole(UserRoleEnum value)
  {
    Value = value;
  }
  
  public static UserRole From(UserRoleEnum role) => new(role);
  public static UserRole User() => new(UserRoleEnum.User);
  public static UserRole Admin() => new(UserRoleEnum.Admin);

  public static implicit operator UserRole(UserRoleEnum role) => new UserRole(role);
  public bool Equals(UserRole? other) => other != null && Value == other.Value;
  public override bool Equals(object? obj) => Equals(obj as UserRole);
  public override int GetHashCode() => Value.GetHashCode();
  
  public override string ToString() => Value.ToString();

};