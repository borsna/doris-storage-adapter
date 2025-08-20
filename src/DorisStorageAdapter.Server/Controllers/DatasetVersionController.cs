﻿using DorisStorageAdapter.Server.Controllers.Authorization;
using DorisStorageAdapter.Services.Contract;
using DorisStorageAdapter.Services.Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace DorisStorageAdapter.Server.Controllers;

[ApiController]
public sealed class DatasetVersionController(IDatasetVersionService service) : ControllerBase
{
    private readonly IDatasetVersionService service = service;

    [HttpPut("{identifier}/{version}/publish")]
    [Authorize(Roles = Roles.Service)]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
    public async Task<Results<Ok, ForbidHttpResult>> Publish(
        string identifier,
        string version,
        [FromForm(Name = "access_right")] AccessRight accessRight,
        [FromForm(Name = "canonical_doi")] string canonicalDoi,
        [FromForm] string doi,
        CancellationToken cancellationToken)
    {
        var datasetVersion = new DatasetVersion(identifier, version);

        if (!CheckClaims(datasetVersion))
        {
            return TypedResults.Forbid();
        }

        await service.Publish(datasetVersion, accessRight, canonicalDoi, doi, cancellationToken);

        return TypedResults.Ok();
    }

    [HttpPut("{identifier}/{version}/status")]
    [Authorize(Roles = Roles.Service)]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
    public async Task<Results<Ok, ForbidHttpResult>> SetStatus(
        string identifier, 
        string version,
        [FromForm] DatasetVersionStatus status,
        CancellationToken cancellationToken)
    {
        var datasetVersion = new DatasetVersion(identifier, version);

        if (!CheckClaims(datasetVersion))
        {
            return TypedResults.Forbid();
        }

        await service.SetStatus(datasetVersion, status, cancellationToken);

        return TypedResults.Ok();
    }

    private bool CheckClaims(DatasetVersion datasetVersion) =>
       Claims.CheckClaims(datasetVersion, User.Claims);
}
