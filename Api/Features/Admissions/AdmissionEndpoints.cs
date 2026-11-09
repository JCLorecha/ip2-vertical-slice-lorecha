namespace Api.Features.Admissions;

using Api.Common.Authorization;
using Api.Common.Extensions;
using Application.Abstractions.Messaging;
using Application.Features.Admissions.CreateAdmission;
using Application.Features.Admissions.GetAdmissionById;
using Application.Features.Admissions.GetAllAdmissions;
using Domain.Common;

public static class AdmissionEndpoints
{
    private const string GetByIdRouteName = "GetAdmissionById";
    public static void MapAdmissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admissions")
            .WithTags("Admissions")
            .RequireAuthorization(); // every endpoint below now needs a token

        // AdmissionEndpoints.cs — only a Registrar creates/lists admissions; GetById is fixed in Step 14
        group.MapPost("/", Create).RequireAuthorization(Policies.CanManageAdmissions);
        group.MapGet("/", GetAll).RequireAuthorization(Policies.CanManageAdmissions);
        group.MapGet("/{id:guid}", GetById).RequireAuthorization();
    }

    private static async Task<IResult> Create(
        CreateAdmissionCommand command,
        ICommandHandler<CreateAdmissionCommand, Result<CreateAdmissionResponse>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, "GetByIdRouteName", new { id = result.Value!.Id })
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetById(
        Guid id,
        IQueryHandler<GetAdmissionByIdQuery, Result<GetAdmissionByIdResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAdmissionByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
        private static async Task<IResult> GetAll(
        IQueryHandler<GetAllAdmissionsQuery, Result<GetAllAdmissionsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllAdmissionsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value) : result.ToProblemDetails();
    }

}

