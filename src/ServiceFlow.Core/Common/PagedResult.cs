namespace ServiceFlow.Core.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Pagesize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Pagesize);
}