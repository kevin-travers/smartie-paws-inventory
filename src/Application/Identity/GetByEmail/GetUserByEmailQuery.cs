using Application.Abstractions.Messaging;

namespace Application.Identity.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
