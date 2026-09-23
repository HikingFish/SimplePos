using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Commands.ActivateProduct;
using SimplePos.Application.Products.Commands.AddProductTax;
using SimplePos.Application.Products.Commands.CreateProduct;
using SimplePos.Application.Products.Commands.DeleteProduct;
using SimplePos.Application.Products.Commands.RemoveProductTax;
using SimplePos.Application.Products.Queries.ProductByProductId;
using SimplePos.Application.Products.Queries.ProductListByCompanyId;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Users;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace SimplePos.WebApi.EndPoints;
public static class ProductEndpoints
{
    public record GetProductsQueryRequest(
        int Page = 1,
        int PageSize = 10,
        string SortBy = "ProductName",
        bool IsDescending = false
    );

    public record CreateProductQueryRequest(
        Guid? CategoryId,
        string? SKU,
        string ProductName,
        decimal? CostPrice,
        decimal BasePrice,
        Guid[]? TaxIds
     );

    public record AddProductTaxRequest(Guid TaxId);

    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (
                [AsParameters] GetProductsQueryRequest request,
                ClaimsPrincipal user,
                ICqrsDispatcher dispatcher,
                CancellationToken ct) =>
        {
            string? CompanyIdClaim = user.FindFirst("CompanyId")?.Value;

            if(!Guid.TryParse(CompanyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetProductByCompanyIdQuery(
                companyId,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.IsDescending
            );
            var result = await dispatcher.QueryAsync<Result<PagedList<ProductListItemResponse>>>(query, ct);
            if (result.IsSuccess)
                return Results.Ok(result.Data);
            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<PagedList<ProductListItemResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        group.MapPost("/", async(
            CreateProductQueryRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
            {
                string companyIdClaim = user.FindFirst("CompanyId")?.Value ?? string.Empty;

                if (!Guid.TryParse(companyIdClaim, out var companyId))
                {
                    return Results.Unauthorized();
                }

                CreateProductCommand createProductCommand = new CreateProductCommand(companyId, request.CategoryId, request.SKU, request.ProductName, request.CostPrice, request.BasePrice, request.TaxIds);
                Result<Guid> result = await dispatcher.SendAsync<CreateProductCommand, Result<Guid>>(createProductCommand, ct);

                return Results.Created($"/api/products/{result.Data}",
                new { id = result.Data });
            })
            .RequireAuthorization();

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
            {
                string? companyIdClaim = user.FindFirst("CompanyId")?.Value;

                if (!Guid.TryParse(companyIdClaim, out var companyId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetProductByProductIdQuery(id, companyId);
                var result = await dispatcher.QueryAsync<Result<ProductResponse>>(query, ct);

                if (result.IsSuccess)
                    return Results.Ok(result.Data);

                if (result.Error.Type == ErrorType.NotFound)
                    return Results.NotFound(new { detail = result.Error.Description });

                return Results.Problem(detail: result.Error.Description, statusCode: 500);
            })
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;

            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteProductCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeleteProductCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .RequireAuthorization();

        group.MapPatch("/{id:guid}/activate", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;

            if(!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new ActivateProductCommand(id, companyId);
            Result result = await dispatcher.SendAsync<ActivateProductCommand, Result>(command, ct);
            
            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 6. POST /api/products/{id:guid}/taxes - Add tax to product
        group.MapPost("/{id:guid}/taxes", async (
            Guid id,
            AddProductTaxRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new AddProductTaxCommand(id, companyId, request.TaxId);
            Result result = await dispatcher.SendAsync<AddProductTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. DELETE /api/products/{id:guid}/taxes/{taxId:guid} - Remove tax from product
        group.MapDelete("/{id:guid}/taxes/{taxId:guid}", async (
            Guid id,
            Guid taxId,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new RemoveProductTaxCommand(id, companyId, taxId);
            Result result = await dispatcher.SendAsync<RemoveProductTaxCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
