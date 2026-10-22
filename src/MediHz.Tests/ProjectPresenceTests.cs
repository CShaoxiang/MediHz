using MediHz.Api.Hubs;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace MediHz.Tests;

public class ProjectPresenceTests
{
    [Fact]
    public void MultipleTabs_StayOnlineUntilLastDisconnect_AndProjectsAreIsolated()
    {
        var tracker = new ProjectPresenceTracker();
        var project = Guid.NewGuid();
        var otherProject = Guid.NewGuid();
        var user = Guid.NewGuid();
        var otherUser = Guid.NewGuid();
        tracker.Connect("tab1", project, user);
        tracker.Connect("tab2", project, user);
        tracker.Connect("other", otherProject, otherUser);
        var snapshot = tracker.Disconnect("tab1")!;
        Assert.Equal(project, snapshot.ProjectId);
        Assert.Equal(user, Assert.Single(snapshot.OnlineUserIds));
        var offline = tracker.Disconnect("tab2")!;
        Assert.Empty(offline.OnlineUserIds);
        Assert.True(offline.Version > snapshot.Version);
        Assert.Null(tracker.Disconnect("tab2"));
        Assert.Empty(tracker.Disconnect("other")!.OnlineUserIds);
    }

    [Fact]
    public async Task UnauthorizedProject_CannotJoinOrPublishPresence()
    {
        var projectId = Guid.NewGuid();
        var access = new Mock<IProjectAccessService>();
        access.Setup(a => a.CheckProjectAsync(projectId, ProjectPermission.Read, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProblemDetails { Status = 404, Detail = "Access denied." });
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
        var tracker = new ProjectPresenceTracker();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString($"?projectId={projectId}");
        var features = new FeatureCollection();
        var httpFeature = new Mock<IHttpContextFeature>();
        httpFeature.SetupGet(f => f.HttpContext).Returns(httpContext);
        features.Set(httpFeature.Object);
        var context = new Mock<HubCallerContext>();
        context.SetupGet(c => c.Features).Returns(features);
        context.SetupGet(c => c.ConnectionId).Returns("denied");
        var hub = new ProjectPresenceHub(access.Object, currentUser.Object, tracker)
        {
            Context = context.Object,
            Groups = Mock.Of<IGroupManager>(),
            Clients = Mock.Of<IHubCallerClients>(),
        };
        await Assert.ThrowsAsync<HubException>(() => hub.OnConnectedAsync());
        Assert.Null(tracker.Disconnect("denied"));
        Mock.Get(hub.Groups).VerifyNoOtherCalls();
        Mock.Get(hub.Clients).VerifyNoOtherCalls();
    }
}
