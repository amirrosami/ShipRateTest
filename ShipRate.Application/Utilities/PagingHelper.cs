namespace ShipRate.Application.Utilities
{
    public static class PagingHelper
    {
        public static List<int> GetVisiblePages(int currentPage, int totalPages, int maxPagesToShow = 5)
        {
            if (totalPages <= 0)
                return new List<int>();

            if (maxPagesToShow < 3)
                maxPagesToShow = 3;

            int half = maxPagesToShow / 2;
            int startPage = currentPage - half;
            int endPage = currentPage + half;

            if (startPage < 1)
            {
                endPage += (1 - startPage);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                startPage -= (endPage - totalPages);
                endPage = totalPages;

                if (startPage < 1)
                    startPage = 1;
            }

            return Enumerable.Range(startPage, endPage - startPage + 1).ToList();
        }
    }
}
