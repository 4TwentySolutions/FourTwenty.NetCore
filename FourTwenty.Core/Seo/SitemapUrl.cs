using System;

namespace FourTwenty.Core.Seo
{
    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }

    public class SitemapUrl
    {
        public string Url { get; set; }
        public DateTime? Modified { get; set; }
        public ChangeFrequency? ChangeFrequency { get; set; }
        public double? Priority { get; set; }
        public SitemapLink[]? Links { get; set; }
    }

    public class SitemapLink
    {
        public string Href { get; set; }
        public string HrefLang { get; set; }
        public string Rel { get; set; }
    }
}
