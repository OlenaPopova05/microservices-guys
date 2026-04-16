using System.ComponentModel.DataAnnotations;

namespace WorkflowService.Api.Requests;

public class StartWorkflowRequest
{
    [Required]
    public Guid OwnerUserId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool ForceStep3Failure { get; set; }
}
