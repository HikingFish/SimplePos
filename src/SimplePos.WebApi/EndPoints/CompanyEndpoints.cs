using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Application.Companies.Queries;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.WebApi.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies").WithTags("Companies");

        group.MapPost("/", async (CreateCompanyCommand command, ICqrsDispatcher dispatcher, CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<CreateCompanyCommand, Result>(command, ct);
            if (result.IsSuccess)
                return Results.Created();
            if (result.Error.Type == ErrorType.Validation)
                return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
            return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (Guid id, ICqrsDispatcher dispatcher, CancellationToken ct) =>
        {
            GetCompanyByIdQuery query = new GetCompanyByIdQuery(id);
            var result = await dispatcher.QueryAsync<Result<CompanyResponse>>(query, ct);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Data);
            }

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(title: "Error", detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization("Admin");
    }
}