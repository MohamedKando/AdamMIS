namespace AdamMIS.Entities.ReportsEnitites
{
    public class RCategories
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Reports> Reports { get; set; } = new List<Reports>();

    }
}
