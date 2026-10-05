using System;

namespace GSEvent.Common.Pagination;

public class PaginationRequest
{
    public int Page {get; set;}
    public int PageSize {get; set;}
    public string? Search {get; set;}
    public string? SortBy {get; set;}
    public string? SortDirection {get; set;} 
}
