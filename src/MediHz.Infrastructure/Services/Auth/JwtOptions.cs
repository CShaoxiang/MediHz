namespace MediHz.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";
    
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
    //Token expires in : 1hrs
    public int Expiration { get; init; } = 60 * 24;
}