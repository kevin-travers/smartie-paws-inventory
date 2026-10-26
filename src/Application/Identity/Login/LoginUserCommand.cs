using Application.Abstractions.Messaging;

namespace Application.Identity.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<AccessTokensResponse>;
