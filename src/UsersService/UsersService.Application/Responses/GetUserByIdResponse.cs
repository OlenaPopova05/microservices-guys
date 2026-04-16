namespace UsersService.Application.Responses;

public record GetUserByIdResponse(Guid UserId, string DisplayName);