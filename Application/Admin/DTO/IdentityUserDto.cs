namespace Application.Admin.DTO;

public class IdentityUserDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
