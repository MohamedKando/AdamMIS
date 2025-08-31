using AdamMIS.Entities.ReportsEnitites;

namespace AdamMIS.Entities.MetaBase
{
    public class Metabase
    {
        public int Id {  get; set; }
        public string Url { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public ICollection<UsersMetabases> UserMetabase { get; set; } = new List<UsersMetabases>();
    }
}
