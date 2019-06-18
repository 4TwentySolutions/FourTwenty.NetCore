namespace FourTwenty.Dashboard.Areas.Dashboard.Models
{
    public class Navbar
    {
        public int Id { get; set; }
        public string NameOption { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string ImageClass { get; set; }
        public string Activeli { get; set; }
        public bool Restricted { get; set; }
        public string[] Roles { get; set; }
        public bool IsDropdown { get; set; }
        public int ParentId { get; set; }
        public object Parameters { get; set; }
    }
}
