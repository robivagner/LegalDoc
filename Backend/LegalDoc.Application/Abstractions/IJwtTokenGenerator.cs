using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(IdentityUser user, IList<string> roles);
}