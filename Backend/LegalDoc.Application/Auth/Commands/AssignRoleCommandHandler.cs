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
            throw new ArgumentException($"Rolul '{request.RoleName}' nu este valid pentru această operațiune.");
        
        if (!await roleManager.RoleExistsAsync(request.RoleName))
            throw new InvalidOperationException("Rolul nu există în baza de date.");
        
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        
        var result = await userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            
            throw new InvalidOperationException($"Eroare la asignarea rolului: {errors}");
        }

        return Unit.Value;
    }
}