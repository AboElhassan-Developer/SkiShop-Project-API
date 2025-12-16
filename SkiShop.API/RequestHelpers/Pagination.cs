namespace SkiShop.API.RequestHelpers
{
    public class Pagination<T> (int pageIndex, int pageSize, int Count, IReadOnlyList<T> data)
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public int Count { get; } = Count;
        public IReadOnlyList<T> Data { get; } = data;

    }
}
