using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Commands;

public class RegisterCommandHandler(UserManager<IdentityUser> userManager) : IRequestHandler<RegisterCommand, Unit>
{
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser { UserName = request.UserName};
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, "Viewer");
        
        return Unit.Value;
    }
}