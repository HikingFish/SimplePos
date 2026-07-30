using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Companies.Commands.CreateCompany;
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
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Error);
        });
    }
}