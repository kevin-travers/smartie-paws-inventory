using Application.Abstractions.Messaging;

namespace Application.Identity.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AccessTokensResponse>;
