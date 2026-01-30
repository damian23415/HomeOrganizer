namespace HomeOrganizer.Application.Features.Repositories;

public interface IPasswordHasher
{
  string HashPassword(string password);
  bool VerifyPassword(string password, string hash);
}