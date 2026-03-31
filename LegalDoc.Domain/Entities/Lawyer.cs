namespace LegalDoc.Domain.Entities;

public class Lawyer
{
    private Lawyer()
    {
        
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string BarNumber { get; private set; } = null!;
    public string? Email { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public static Lawyer Create(string name, string barNumber, string? email)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Lawyer name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(barNumber))
        {
            throw new ArgumentException("Lawyer bar number cannot be null or empty.", nameof(barNumber));
        }

        return new Lawyer
        {
            Id = Guid.NewGuid(),
            Name = name,
            BarNumber = barNumber,
            Email = email,
            IsActive = true
        };
    }
    
    public void UpdateLawyerActivity(bool isActive)
    {
        if(IsActive == isActive)
            throw new InvalidOperationException("Cannot update to the same Active state.");
        
        IsActive = isActive;
    }
}