using System.ComponentModel.DataAnnotations;

namespace UsersService.Api.User;

public class CreateUserRequest
{
    [Required(ErrorMessage = "User name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "User name must be between 1 and 100 characters")]
    public string DisplayName { get; set; } = null!;
}