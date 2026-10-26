using Domain.Identity;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);

    string GenerateRefreshToken();
}
