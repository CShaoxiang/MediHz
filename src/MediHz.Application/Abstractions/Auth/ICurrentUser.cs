namespace MediHz.Application.Abstractions.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
