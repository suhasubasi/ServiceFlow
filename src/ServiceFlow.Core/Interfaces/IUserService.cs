using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Interfaces;

public interface IUserService
{
    Task<User?> ValidateCredentialsAsync(string username, string password);
}