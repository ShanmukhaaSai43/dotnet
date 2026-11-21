namespace EmployeeManagement.Application.DTOs;

public record PaginationParams(
    int PageNumber = 1,
    int PageSize = 10,
    int? DepartmentId = null,
    decimal? MinPay = null,
    string? SearchTerm = null,
    string? SortBy = "name",
    string? SortDirection = "asc"
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
)
{
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        int totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        return new PagedResult<T>(
            Items: items,
            TotalCount: totalCount,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: totalPages,
            HasPreviousPage: pageNumber > 1,
            HasNextPage: pageNumber < totalPages
        );
    }
}
