using LegalDoc.Application.Abstractions;
using LegalDoc.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Commands;

public class ChangeUsernameCommandHandler(UserManager<IdentityUser> userManager, IJwtTokenGenerator tokenService) 
    : IRequestHandler<ChangeUsernameCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null) 
        {
            throw new KeyNotFoundException("Utilizatorul nu a fost găsit.");
        }
    
        var existingUser = await userManager.FindByNameAsync(request.NewUsername);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            throw new InvalidOperationException("Acest nume de utilizator este deja utilizat.");
        }

        var result = await userManager.SetUserNameAsync(user, request.NewUsername);

        if (!result.Succeeded) 
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Eroare la actualizarea numelui de utilizator: {errors}");
        }
    
        await userManager.UpdateNormalizedUserNameAsync(user);
        
        var roles = await userManager.GetRolesAsync(user);
        var newToken = tokenService.GenerateToken(user, roles);
        
        return new AuthResponse(newToken, request.NewUsername);
    }
}