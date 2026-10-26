using Application.Abstractions.Messaging;

namespace Application.Identity.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
