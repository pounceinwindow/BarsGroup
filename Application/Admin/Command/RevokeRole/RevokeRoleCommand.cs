using MediatR;

namespace Application.Admin.Command.RevokeRole;

public record RevokeRoleCommand(string Username, int RevokedBy) : IRequest;
