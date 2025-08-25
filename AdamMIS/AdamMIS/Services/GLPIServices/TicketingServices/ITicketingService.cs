

namespace AdamMIS.Services.GLPIServices.TicketingServices
{

        public interface ITicketingService
        {
            Task<string> LoginAsync();
            Task<TicketResponse> CreateTicketAsync(CreateTicket ticketDto);
            Task<TicketResponse> GetTicketAsync(int ticketId);
            Task<List<TicketResponse>> GetAllTicketsAsync();
        Task<TicketResponse> SolveTicketAsync(int ticketId);
        Task<TicketResponse> CloseTicketAsync(int ticketId);

    }
    
}
