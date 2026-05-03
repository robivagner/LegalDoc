using LegalDoc.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class RegisterCommandHandler(UserManager<IdentityUser> userManager) : IRequestHandler<RegisterCommand, Unit>
{
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser { UserName = request.UserName, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, "Viewer");
        
        return Unit.Value;
    }
}