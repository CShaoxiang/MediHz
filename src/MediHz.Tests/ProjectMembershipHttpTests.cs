using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MediHz.Api.Controllers;
using MediHz.Application;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.Abstractions.Users;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Projects;
using MediHz.Domain.Users;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Persistence.Services.Annotations;
using MediHz.Infrastructure.Persistence.Services.Projects;
using MediHz.Infrastructure.Services;
using MediHz.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace MediHz.Tests;

public class ProjectMembershipHttpTests
{
    [Theory]
    [InlineData("Annotator")]
    [InlineData("Viewer")]
    public async Task TwoAccounts_ExerciseRealRoutingBindingAuthenticationAndMembership(string role)
    {
        // Isolated HTTP server and database: never contacts Google or a development/production database.
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddControllers().AddApplicationPart(typeof(ProjectsController).Assembly);
        builder.Services.AddApplication(builder.Configuration);
        var database = Guid.NewGuid().ToString();
        builder.Services.AddDbContext<MediHzDbContext>(options =>
            options.UseInMemoryDatabase(database)
        );
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAudioFileService, AudioFileService>();
        builder.Services.AddScoped<IAnnotationService, AnnotationService>();
        builder.Services.AddSingleton(Mock.Of<IAudioFileDeleteService>());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                "membership-tests-only-signing-key-not-for-production-1234567890"
            )
        );
        builder
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "membership-tests",
                    ValidateAudience = true,
                    ValidAudience = "membership-tests",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                };
            });
        builder.Services.AddAuthorization();
        await using var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        var owner = new User
        {
            Email = "alice@example.com",
            DisplayName = "Alice",
            ProviderToken = "alice",
        };
        var member = new User
        {
            Email = "bob@example.com",
            DisplayName = "Bob",
            ProviderToken = "bob",
        };
        var project = new Project("HTTP acceptance project", owner.Id);
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MediHzDbContext>();
            db.AddRange(owner, member, project);
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        using var ownerClient = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        using var memberClient = new HttpClient { BaseAddress = ownerClient.BaseAddress };
        using var anonymous = new HttpClient { BaseAddress = ownerClient.BaseAddress };
        string Token(Guid userId) =>
            new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    issuer: "membership-tests",
                    audience: "membership-tests",
                    claims: [new Claim("sub", userId.ToString())],
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                )
            );
        ownerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Token(owner.Id)
        );
        memberClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Token(member.Id)
        );
        var url = $"/api/Projects/{project.Id}";

        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync(url)).StatusCode);
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (
                await anonymous.PostAsJsonAsync(
                    url + "/members",
                    new { email = member.Email, role }
                )
            ).StatusCode
        );
        Assert.Empty((await memberClient.GetFromJsonAsync<ProjectDto[]>("/api/Projects/all"))!);
        Assert.Equal(HttpStatusCode.NotFound, (await memberClient.GetAsync(url)).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await memberClient.GetAsync(url + "/files")).StatusCode
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await ownerClient.PostAsJsonAsync(url + "/members", new { email = "invalid", role })
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await ownerClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = member.Email, role = "Owner" }
                )
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await ownerClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = member.Email, role = "Invalid" }
                )
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await ownerClient.PostAsJsonAsync(url + "/members", new { email = member.Email })
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (
                await ownerClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = "unknown@example.com", role }
                )
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.Conflict,
            (
                await ownerClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = owner.Email, role }
                )
            ).StatusCode
        );

        using var added = await ownerClient.PostAsJsonAsync(
            url + "/members",
            new { email = "  BOB@EXAMPLE.COM  ", role }
        );
        Assert.Equal(HttpStatusCode.Created, added.StatusCode);
        using var body = JsonDocument.Parse(await added.Content.ReadAsStringAsync());
        Assert.Equal(role, body.RootElement.GetProperty("role").GetString());
        Assert.Equal(member.Id, body.RootElement.GetProperty("userId").GetGuid());
        Assert.Equal("bob@example.com", body.RootElement.GetProperty("email").GetString());
        Assert.Equal(
            HttpStatusCode.Conflict,
            (
                await ownerClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = member.Email, role }
                )
            ).StatusCode
        );

        Assert.Equal(
            project.Id,
            Assert
                .Single((await memberClient.GetFromJsonAsync<ProjectDto[]>("/api/Projects/all"))!)
                .Id
        );
        Assert.Equal(role, (await memberClient.GetFromJsonAsync<ProjectDto>(url))!.Role.ToString());
        Assert.Equal(HttpStatusCode.OK, (await memberClient.GetAsync(url + "/files")).StatusCode);
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (
                await memberClient.PostAsJsonAsync(
                    url + "/members",
                    new { email = owner.Email, role }
                )
            ).StatusCode
        );
        // Supplying the owner's ID in an old route cannot impersonate the owner.
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (
                await memberClient.PatchAsJsonAsync(url + "/" + owner.Id, new { name = "Stolen" })
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await memberClient.DeleteAsync(url + "/" + owner.Id)).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.OK,
            (
                await ownerClient.PatchAsJsonAsync(url + "/" + member.Id, new { name = "Renamed" })
            ).StatusCode
        );
        await app.StopAsync();
    }
}
