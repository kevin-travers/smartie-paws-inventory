using SharedKernel;

namespace Domain.Identity;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email) : IDomainEvent;
