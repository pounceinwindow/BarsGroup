using MediatR;

namespace Application.Admin.Command.AssignRole;

public record AssignRoleCommand(string Username, string RoleName) : IRequest;
