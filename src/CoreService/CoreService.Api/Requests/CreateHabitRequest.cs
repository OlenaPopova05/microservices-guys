using System.ComponentModel.DataAnnotations;

namespace CoreService.Api.Requests;

public class CreateHabitRequest
{
    [Required(ErrorMessage = "Owner user id is required")]
    public Guid OwnerUserId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = null!;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }
}
