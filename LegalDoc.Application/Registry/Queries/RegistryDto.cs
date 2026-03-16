namespace LegalDoc.Application.Registry.Queries;

public record RegistryDto(Guid Id, string Name, string Location, int Capacity, int Availability);