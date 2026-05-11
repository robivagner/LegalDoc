using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Commands;

public class LoginCommandHandler(UserManager<IdentityUser> userManager, IJwtTokenGenerator tokenGenerator) 
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
    
        var roles = await userManager.GetRolesAsync(user);
        var token = tokenGenerator.GenerateToken(user, roles);
        
        return new AuthResponse(token, user.UserName ?? request.UserName);
    }
}