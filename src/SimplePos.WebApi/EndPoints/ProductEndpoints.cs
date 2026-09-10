using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Commands.CreateProduct;
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
            var result = await dispatcher.QueryAsync<Result<PagedList<ProductResponse>>>(query, ct);
            if (result.IsSuccess)
                return Results.Ok(result.Data);
            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<PagedList<ProductResponse>>(StatusCodes.Status200OK)
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
    }
}
