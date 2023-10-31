namespace APICatalogo.Pagination;

public class QueryStringParameters
{
    const int maxPageSize = 250;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 30;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? value : maxPageSize;
        }
    }
}
