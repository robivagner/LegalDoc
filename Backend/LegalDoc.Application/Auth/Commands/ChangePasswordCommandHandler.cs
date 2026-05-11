using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Commands;

public class ChangePasswordCommandHandler(UserManager<IdentityUser> userManager) 
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null) 
        {
            throw new KeyNotFoundException("Utilizatorul nu a fost găsit.");
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            
            throw new InvalidOperationException($"Eroare la schimbarea parolei: {errors}");
        }
    }
}