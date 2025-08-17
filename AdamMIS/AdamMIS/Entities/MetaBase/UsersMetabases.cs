using AdamMIS.Entities.ReportsEnitites;

namespace AdamMIS.Entities.MetaBase
{
    public class UsersMetabases
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int MetabaseId { get; set; }

        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public Metabase MetaBase { get; set; } = default!;
    }
}
