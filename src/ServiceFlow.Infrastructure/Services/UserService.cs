using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Infrastructure.Persistence;

namespace ServiceFlow.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ServiceFlowDbContext _context;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(ServiceFlowDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }
}