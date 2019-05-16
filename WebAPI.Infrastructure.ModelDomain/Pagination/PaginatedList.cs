using System.Collections.Generic;

namespace WebAPI.Infrastructure.DomainModel.Pagination
{
    public class PaginatedList<T> : List<T> where T : class
    {
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }

        private int _totalItemCount;

        public int TotalItemCount
        {
            get => _totalItemCount;
            set => _totalItemCount = value > 0 ? value : 0;
        }

        public int PageCount => TotalItemCount / PageSize + (TotalItemCount % PageSize > 0 ? 1 : 0);

        public bool HasPrevious => PageIndex > 0;

        public bool HasNext => PageIndex < PageCount - 1;

        public PaginatedList(int pageIndex, int pageSize, int totalItemCount, List<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            AddRange(data);
        }
    }
}