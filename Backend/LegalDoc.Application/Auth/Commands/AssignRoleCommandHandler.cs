using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Commands;

public class AssignRoleCommandHandler(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) 
    : IRequestHandler<AssignRoleCommand, Unit>
{
    public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) throw new KeyNotFoundException("Utilizatorul nu a fost găsit.");
        
        var validRoles = new[] { "Lawyer", "Viewer" };
        if (!validRoles.Contains(request.RoleName))
            throw new Exception("Rolul specificat nu este valid.");
        
        if (!await roleManager.RoleExistsAsync(request.RoleName))
            throw new Exception("Rolul nu există în baza de date.");
        
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        
        var result = await userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            throw new Exception("Eroare la asignarea rolului.");

        return Unit.Value;
    }
}