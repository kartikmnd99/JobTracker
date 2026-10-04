using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(User user);
}