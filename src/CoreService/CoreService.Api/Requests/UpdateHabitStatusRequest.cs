using System.ComponentModel.DataAnnotations;

namespace CoreService.Api.Requests;

public class UpdateHabitStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Status must be between 3 and 20 characters")]
    public string Status { get; set; } = null!;
}
