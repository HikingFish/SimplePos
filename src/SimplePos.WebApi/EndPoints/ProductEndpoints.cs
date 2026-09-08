using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Common;
using SimplePos.Application.Products.Queries.ProductListByCompanyId;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.WebApi.EndPoints;
public static class ProductEndpoints
{
    public record GetProductsQueryRequest(
        Guid CompanyId,
        int Page = 1,
        int PageSize = 10,
        string SortBy = "ProductName",
        bool IsDescending = false
    );
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (
                [AsParameters] GetProductsQueryRequest request,
                ICqrsDispatcher dispatcher,
                CancellationToken ct) =>
        {
            var query = new GetProductByCompanyIdQuery(
                request.CompanyId,
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
    }
}
