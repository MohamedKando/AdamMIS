namespace AdamMIS.Entities.DepartmentEntities
{
    public class DepartmentHead
    {
        public int Id { get; set; }
        public string HeadId { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
