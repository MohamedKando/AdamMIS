namespace AdamMIS.Contract.Tickets
{
    public class CreateTicket
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } = 3; // Default: Medium (1=Very Low, 2=Low, 3=Medium, 4=High, 5=Very High)
        public int Urgency { get; set; } = 3;  // Default: Medium
        public int Type { get; set; } = 1;     // Default: Incident (1=Incident, 2=Request)
        public string? AssignedToName { get; set; } // Optional: Name of technician to assign
        public int? TimeToResolve { get; set; } // Optional: Time to resolve in hours
    }
}
