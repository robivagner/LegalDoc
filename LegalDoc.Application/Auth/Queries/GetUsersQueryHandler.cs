using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LegalDoc.Application.Auth.Queries;

public class GetUsersQueryHandler(UserManager<IdentityUser> userManager)
    : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = userManager.Users;
        
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            query = query.Where(u => u.UserName == request.UserName);
        }

        var users = query.ToList();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Viewer";
            
            result.Add(new UserDto(
                user.Id, 
                user.UserName!,
                primaryRole));
        }

        return result;
    }
}