namespace SimplePos.Application.Common;

public record PagedList<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);