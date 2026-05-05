using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Queries;

public class GetCurrentUserQueryHandler(UserManager<IdentityUser> userManager) 
    : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new KeyNotFoundException("Utilizatorul nu a fost găsit.");
        }
        
        var roles = await userManager.GetRolesAsync(user);
        
        var primaryRole = roles.FirstOrDefault() ?? "Viewer";
        
        return new UserDto(
            user.Id, 
            user.UserName ?? string.Empty, 
            primaryRole
        );
    }
}