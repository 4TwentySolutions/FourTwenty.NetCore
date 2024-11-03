using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FourTwenty.Core.Seo
{
    public class SitemapBuilder
    {
        private readonly XNamespace _ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly XNamespace _xhtml = "http://www.w3.org/1999/xhtml";

        public List<SitemapUrl> Urls { get; }

        public SitemapBuilder()
        {
            Urls = new List<SitemapUrl>();
        }

        public void AddUrl(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null, SitemapLink[]? links = null)
        {
            Urls.Add(new SitemapUrl()
            {
                Url = url,
                Modified = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority,
                Links = links
            });
        }

        public override string ToString()
        {
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(_ns + "urlset", new XAttribute(XNamespace.Xmlns + "xhtml", _xhtml),
                    from item in Urls
                    select CreateItemElement(item)
                ));

            return sitemap.ToString();
        }

        private XElement CreateItemElement(SitemapUrl url)
        {
            XElement itemElement = new XElement(_ns + "url", new XElement(_ns + "loc", url.Url.ToLower()));

            if (url.Modified.HasValue)
            {
                itemElement.Add(new XElement(_ns + "lastmod", url.Modified.Value.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));
            }

            if (url.ChangeFrequency.HasValue)
            {
                itemElement.Add(new XElement(_ns + "changefreq", url.ChangeFrequency.Value.ToString().ToLower()));
            }

            if (url.Priority.HasValue)
            {
                itemElement.Add(new XElement(_ns + "priority", url.Priority.Value.ToString("N1").Replace(",", ".")));
            }

            if (url.Links != null && url.Links.Any())
            {

                foreach (var link in url.Links)
                {
                    var langElement = new XElement(_xhtml + "link", new XAttribute("rel", link.Rel), new XAttribute("hreflang", link.HrefLang), new XAttribute("href", link.Href));
                    itemElement.Add(langElement);
                }
            }

            return itemElement;
        }
    }
}
