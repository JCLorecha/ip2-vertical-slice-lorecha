namespace Api.Features.Students;

using Api.Common.Authorization;
using Api.Common.Extensions;
using Application.Abstractions.Messaging;
using Application.Features.Students.CreateStudent;
using Application.Features.Students.GetAllStudents;
using Application.Features.Students.GetStudentById;
using Domain.Common;

public static class StudentEndpoints
{
    private const string GetByIdRouteName = "GetStudentById";
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/students")
            .WithTags("Students")
            .RequireAuthorization(); // every endpoint below now needs a token

        // StudentEndpoints.cs — a student browsing other students is out of scope for this lab
        group.MapGet("/", GetAll).RequireAuthorization(Policies.CanManageStudents);
        group.MapGet("/{id:guid}", GetById).RequireAuthorization(Policies.CanManageStudents);
        group.MapPost("/", Create).RequireAuthorization(Policies.CanManageStudents);
    }

    private static async Task<IResult> GetAll(
        IQueryHandler<GetAllStudentsQuery, Result<GetAllStudentsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllStudentsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Create(
        CreateStudentCommand command,
        ICommandHandler<CreateStudentCommand, Result<CreateStudentResponse>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, "GetStudentById", new { id = result.Value!.Id })
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetById(
        Guid id,
        IQueryHandler<GetStudentByIdQuery, Result<GetStudentByIdResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
