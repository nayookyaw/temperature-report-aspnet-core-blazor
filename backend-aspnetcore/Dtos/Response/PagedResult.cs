
namespace BackendAspNetCore.Dtos.Response;

public class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
}
