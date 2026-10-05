using System;

namespace GSEvent.Common.Pagination;

public class PaginationResponse<T>
{
    public IList<T> Data {get; set;} = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
