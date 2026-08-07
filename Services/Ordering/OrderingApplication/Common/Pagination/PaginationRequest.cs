namespace OrderingApplication.Common.Pagination;

public class PaginationRequest
{
    private const int MaxPageSize = 100;

    public int PageIndex { get; }
    public int PageSize { get; }

    public PaginationRequest(int pageIndex = 0, int pageSize = 10)
    {
        PageIndex = pageIndex < 0 ? 0 : pageIndex;
        PageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, MaxPageSize);
    }
}
