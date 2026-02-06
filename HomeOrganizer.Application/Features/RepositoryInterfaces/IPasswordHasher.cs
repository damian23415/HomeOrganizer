namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IPasswordHasher
{
  string HashPassword(string password);
  bool VerifyPassword(string password, string hash);
}