namespace FourTwenty.Core.Interfaces
{
    public interface IFilterData
    {
        int? Page { get; set; }
        string Sidx { get; set; }
        string Sord { get; set; }
        int PageSize { get; set; }
        string CustomFilter { get; set; }

        string Ordering { get; }
    }
}
