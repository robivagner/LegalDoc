namespace LegalDoc.Application.Auth.Queries;

public record UserDto(
    string Id, 
    string UserName, 
    string Role
);