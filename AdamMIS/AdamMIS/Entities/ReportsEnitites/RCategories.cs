namespace AdamMIS.Entities.ReportsEnitites
{
    public class RCategories
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Color { get ; set; } = string.Empty;
        public ICollection<Reports> Reports { get; set; } = new List<Reports>();

    }
}
