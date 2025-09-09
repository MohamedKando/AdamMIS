namespace AdamMIS.Contract.Metabase
{
    public class DuplicateAssignment
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int MetabaseId { get; set; }
        public string MetabaseTitle { get; set; }
        public DateTime ExistingAssignedAt { get; set; }
        public string ExistingAssignedBy { get; set; }
    }
}
