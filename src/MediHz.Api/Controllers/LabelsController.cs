using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LabelsController(
    IGetAllLabelGroupsHandler getAllLabelGroupsHandler,
    ICreateLabelGroupHandler createLabelGroupHandler,
    IDeleteLabelGroupHandler deleteLabelGroupHandler,
    IGetLabelsByGroupHandler getLabelsByGroupHandler,
    ICreateLabelHandler createLabelHandler,
    IDeleteLabelHandler deleteLabelHandler,
    IAssignLabelToGroupHandler assignLabelToGroupHandler
) : ControllerBase
{
    [HttpGet("groups")]
    [ProducesResponseType(typeof(IEnumerable<LabelGroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLabelGroups(CancellationToken cancellationToken)
    {
        var response = await getAllLabelGroupsHandler.Handle(new(), cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpPost("groups")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabelGroupDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateLabelGroup(
        [FromForm] CreateLabelGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await createLabelGroupHandler.Handle(request, cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpDelete("groups/{labelGroupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLabelGroup(
        Guid labelGroupId,
        CancellationToken cancellationToken
    )
    {
        var response = await deleteLabelGroupHandler.Handle(labelGroupId, cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LabelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLabels(
        [FromQuery] Guid? groupId,
        CancellationToken cancellationToken
    )
    {
        var response = await getLabelsByGroupHandler.Handle(groupId, cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabelDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateLabel(
        [FromForm] string name,
        [FromForm] Guid? groupId,
        CancellationToken cancellationToken
    )
    {
        var response = await createLabelHandler.Handle(
            new CreateLabelRequest(name, groupId),
            cancellationToken
        );
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpDelete("{labelId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLabel(Guid labelId, CancellationToken cancellationToken)
    {
        var response = await deleteLabelHandler.Handle(labelId, cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpPatch("{labelId}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabelDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignLabelToGroup(
        [FromForm] Guid? targetGroupId,
        Guid labelId,
        CancellationToken cancellationToken
    )
    {
        var response = await assignLabelToGroupHandler.Handle(
            new AssignLabelRequest(labelId, targetGroupId),
            cancellationToken
        );
        return StatusCode(response.StatusCode, response.Body);
    }
}
