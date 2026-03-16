namespace LegalDoc.Domain.Entities;

public class Registry
{
    public Registry()
    {
        
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public int Capacity { get; private set; }
    public int Availability { get; private set; }

    public static Registry Create(string name, string location, int capacity)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Department name cannot be empty.", nameof(name));
        }
        
        if (string.IsNullOrEmpty(location))
        {
            throw new ArgumentException("Department location cannot be empty.", nameof(location));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity needs to be greater than 0", nameof(capacity));
        }
        
        return new Registry
        {
            Id = Guid.NewGuid(),
            Name = name,
            Location = location,
            Capacity = capacity,
            Availability = capacity
        };
    }

    public void DocumentAdded()
    {
        if (Availability <= 0)
            throw new InvalidOperationException("Registry is full!");
        
        Availability--;
    }
    
    public void DocumentRemoved()
    {
        if (Availability >= Capacity)
        {
            throw new InvalidOperationException("Disponibilitatea nu poate depasi capacitatea maxima.");
        }

        Availability++;
    }
}