namespace AdamMIS.Contract.Tickets
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int Urgency { get; set; }
        public int Users_Id_Recipient { get; set; } // Requester user ID
        public int Users_Id_Lastupdater { get; set; } // Assigned technician ID
        public string? Time_To_Resolve { get; set; } // Time to resolve

        // Keep as strings since GLPI returns date strings

        public string Date_Creation { get; set; }

        public string? Closedate { get; set; }
        public string? Solvedate { get; set; }

        // Helper properties to parse dates when needed

        public DateTime? ParsedDateCreation => DateTime.TryParse(Date_Creation, out var date) ? date : null;

        public string StatusText => Status switch
        {
            1 => "New",
            2 => "Processing (assigned)",
            3 => "Processing (planned)",
            4 => "Pending",
            5 => "Solved",
            6 => "Closed",
            _ => "Unknown"
        };

        public string PriorityText => Priority switch
        {
            1 => "Very Low",
            2 => "Low",
            3 => "Medium",
            4 => "High",
            5 => "Very High",
            _ => "Unknown"
        };
    }
}
