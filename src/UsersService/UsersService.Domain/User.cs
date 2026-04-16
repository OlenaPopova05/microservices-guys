namespace UsersService.Domain;

public class User
{
    public Guid Id { get; private set; }

    public string DisplayName { get; private set; }
    
    private User() {}
    
    public User(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name cannot be empty.");

        Id = Guid.NewGuid();
        DisplayName = displayName.Trim();
    }
}