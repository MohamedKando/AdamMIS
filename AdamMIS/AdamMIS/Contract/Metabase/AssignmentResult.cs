namespace AdamMIS.Contract.Metabase
{
    public class AssignmentResult
    {
        public List<UserMetabaseResponse> NewAssignments { get; set; } = new List<UserMetabaseResponse>();
        public List<DuplicateAssignment> DuplicateAssignments { get; set; } = new List<DuplicateAssignment>();
        public int TotalRequested { get; set; }
        public int SuccessfulAssignments { get; set; }
        public int DuplicateCount { get; set; }
    }
}
