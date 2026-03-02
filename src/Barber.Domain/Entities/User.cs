namespace Barber.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public string Role { get; private set; } = "client";

    private User() { }

    public User(string name, string email, string password, string role)
    {
        Name = name;
        Email = email;
        Password = password;
        Role = role;
    }
}