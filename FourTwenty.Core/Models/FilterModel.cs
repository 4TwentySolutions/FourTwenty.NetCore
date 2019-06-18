using FourTwenty.Core.Interfaces;

namespace FourTwenty.Core.Models
{
    public class FilterData : IFilterData
    {
        public virtual int? Page { get; set; }
        public virtual string Sidx { get; set; }
        public virtual string Sord { get; set; }
        public virtual int PageSize { get; set; } = 10;
        public virtual string CustomFilter { get; set; }
        public virtual string Ordering => !string.IsNullOrEmpty(Sord) && !string.IsNullOrEmpty(Sidx) ? $"{Sidx} {Sord}" : null;
    }
}
