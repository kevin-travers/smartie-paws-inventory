using Application.Abstractions.Messaging;

namespace Application.Identity.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
