using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Registrations.Commands.RegisterBusiness;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.WebApi.EndPoints;

public static class RegisterBusinessEndpoints
{
    public static void MapRegisterBusinessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/register-business").WithTags("RegisterBusiness");

        group.MapPost("/", async (RegisterBusinessCommand command, ICqrsDispatcher dispatcher, CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<RegisterBusinessCommand, Result>(command, ct);
            if (result.IsSuccess)
                return Results.Created();
            if (result.Error.Type == ErrorType.Validation)
                return Results.Problem(type: "blank", title: "Bad Request", detail: result.Error.Description, statusCode: 400);
            return Results.Problem(type: "blank", title: "Error", detail: result.Error.Description, statusCode: 500);
        });
    }
}