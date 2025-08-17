namespace AdamMIS.Contract.Metabase
{
    public class UserMetabaseResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int MetabaseId { get; set; }
        public string MetabaseTitle { get; set; } = string.Empty;
        public string MetabaseUrl { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
