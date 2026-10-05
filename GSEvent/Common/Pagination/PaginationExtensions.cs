using System;
using Microsoft.EntityFrameworkCore;

namespace GSEvent.Common.Pagination;

public static class PaginationExtensions
{
    private const int MaxPageSize = 1000;
    public static async Task<PaginationResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        PaginationRequest request
    )
    {
        request.Page = request.Page < 1 ? 1: request.Page;
        request.PageSize = request.PageSize < 1 ? 10 : request.PageSize;
        if (request.PageSize > MaxPageSize)
        {
            request.PageSize = MaxPageSize;
        }
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems/(double) request.PageSize);
        var data = await query 
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PaginationResponse<T>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = request.Page < totalPages,
            HasPreviousPage = request.Page < totalPages
        };
    }
}
