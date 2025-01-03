

using Calendar.Configuration;

namespace Calendar.Helpers
{
    public class Pagination
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords)
        {
            var totalPages = (int)Math.Ceiling((double)totalRecords / validFilter.PageSize);
            var response = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize)
            {
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            // Add pagination links based on the current filter
            if (validFilter.PageNumber < totalPages)
            {
                response.NextPage = new Uri($"?pageNumber={validFilter.PageNumber + 1}&pageSize={validFilter.PageSize}");
            }

            if (validFilter.PageNumber > 1)
            {
                response.PreviousPage = new Uri($"?pageNumber={validFilter.PageNumber - 1}&pageSize={validFilter.PageSize}");
            }

            response.FirstPage = new Uri($"?pageNumber=1&pageSize={validFilter.PageSize}");
            response.LastPage = new Uri($"?pageNumber={totalPages}&pageSize={validFilter.PageSize}");

            return response;
        }

        internal PagedResponse<List<T>> GetPagination<T>(List<T> listRequests, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = listRequests.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();
            var totalRecords = listRequests.Count;
            return CreatePagedReponse<T>(pagedData, validFilter, totalRecords);
        }
    }
}
