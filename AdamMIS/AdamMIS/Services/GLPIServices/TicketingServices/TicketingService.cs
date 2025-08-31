using System.Text;
using System.Text.Json;

namespace AdamMIS.Services.GLPIServices.TicketingServices
{


    public class TicketingService : ITicketingService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _glpiApiUrl = "http://localhost/glpi/apirest.php";
        private readonly string _appToken = "yjSKMUM2XvZSNsNQjX08Fsn20w0tCu2CyrvelFkH";
        private readonly string _userToken = "yQk5zZVCZdee7Akbf0nikgA8Cvz7oyKIPZp8BTsp";
        private string? _sessionToken;

        public TicketingService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            // Fix: Use underscore instead of asterisk
            _httpClient.BaseAddress = new Uri(_glpiApiUrl);
            _httpClient.DefaultRequestHeaders.Add("App-Token", _appToken);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"user_token {_userToken}");
            _httpContextAccessor = httpContextAccessor;
        }

        // 🔑 Login and store session token
        public async Task<string> LoginAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_glpiApiUrl}/initSession");
            request.Headers.Add("App-Token", _appToken);
            request.Headers.Add("Authorization", $"user_token {_userToken}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // Fix: Use underscore instead of asterisk
            _sessionToken = result.GetProperty("session_token").GetString();
            return _sessionToken!;
        }

        // 🎫 Create Ticket
        public async Task<TicketResponse> CreateTicketAsync(CreateTicket ticketDto)
        {
            if (_sessionToken == null)
                await LoginAsync();

            var user = _httpContextAccessor.HttpContext?.User;
            var requesterUsername = user!.GetUsername(); // From your system

            // Build enhanced description with user information
            var enhancedDescription = new StringBuilder();
            enhancedDescription.AppendLine($"**Requester:** {requesterUsername}");

            if (!string.IsNullOrEmpty(ticketDto.AssignedToName))
            {
                enhancedDescription.AppendLine($"**Assigned To:** {ticketDto.AssignedToName}");
            }

            enhancedDescription.AppendLine(); // Empty line for separation
            enhancedDescription.AppendLine("**Description:**");
            enhancedDescription.AppendLine(ticketDto.Description);

            // Calculate time to resolve if provided (convert hours to datetime)
            string? timeToResolve = null;
            if (ticketDto.TimeToResolve.HasValue)
            {
                var resolveDate = DateTime.Now.AddHours(ticketDto.TimeToResolve.Value);
                timeToResolve = resolveDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            var payload = new
            {
                input = new
                {
                    name = $"[{requesterUsername}] {ticketDto.Title}", // Include requester in title
                    content = enhancedDescription.ToString(),
                    priority = ticketDto.Priority,
                    urgency = ticketDto.Urgency,
                    type = ticketDto.Type,
                    time_to_resolve = timeToResolve, // Keep in separate column
                    entities_id = 0 // Adjust entity ID as needed
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_glpiApiUrl}/Ticket")
            {
                Content = content
            };
            request.Headers.Add("Session-Token", _sessionToken);
            request.Headers.Add("App-Token", _appToken);

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            // Debug: Always log the response
            Console.WriteLine($"GLPI Create Ticket Response: {json}");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"GLPI API error: {response.StatusCode} - {json}");
            }

            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

                int ticketId;
                if (jsonElement.ValueKind == JsonValueKind.Array && jsonElement.GetArrayLength() > 0)
                {
                    // Response is an array - get the first element's ID
                    var firstElement = jsonElement[0];
                    if (firstElement.TryGetProperty("id", out var idProperty))
                    {
                        ticketId = idProperty.GetInt32();
                    }
                    else
                    {
                        throw new InvalidOperationException($"No 'id' field found in array response: {json}");
                    }
                }
                else if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    if (jsonElement.TryGetProperty("id", out var idProperty))
                    {
                        // Direct object with ID
                        ticketId = idProperty.GetInt32();
                    }
                    else if (jsonElement.TryGetProperty("data", out var dataProperty) && dataProperty.ValueKind == JsonValueKind.Array)
                    {
                        // Object with data array
                        ticketId = dataProperty[0].GetProperty("id").GetInt32();
                    }
                    else
                    {
                        throw new InvalidOperationException($"No 'id' field found in object response: {json}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected GLPI response format: {json}");
                }

                // Return a basic response instead of fetching the complete ticket
                return new TicketResponse
                {
                    Id = ticketId,
                    Name = $"[{requesterUsername}] {ticketDto.Title}",
                    Content = enhancedDescription.ToString(),
                    Priority = ticketDto.Priority,
                    Urgency = ticketDto.Urgency,
                    Status = 1, // New ticket status
                    Time_To_Resolve = timeToResolve,
                    Date_Creation = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse GLPI response: {json}", ex);
            }
        }

        // 🔍 Get Ticket Info
        public async Task<TicketResponse> GetTicketAsync(int ticketId)
        {
            if (_sessionToken == null)
                await LoginAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_glpiApiUrl}/Ticket/{ticketId}");
            request.Headers.Add("Session-Token", _sessionToken);
            request.Headers.Add("App-Token", _appToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

                // Debug: Log the response structure
                Console.WriteLine($"GLPI GetTicket Response: {json}");

                if (jsonElement.ValueKind == JsonValueKind.Array && jsonElement.GetArrayLength() > 0)
                {
                    // GLPI sometimes returns an array even for single ticket requests
                    var ticketJson = jsonElement[0].GetRawText();
                    return JsonSerializer.Deserialize<TicketResponse>(ticketJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    })!;
                }
                else if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    // Standard single object response
                    return JsonSerializer.Deserialize<TicketResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    })!;
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected GLPI response format: {json}");
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse GLPI ticket response: {json}", ex);
            }
        }

        // 🔍 Get All Tickets
        public async Task<List<TicketResponse>> GetAllTicketsAsync()
        {
            if (_sessionToken == null)
                await LoginAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_glpiApiUrl}/Ticket");
            request.Headers.Add("Session-Token", _sessionToken);
            request.Headers.Add("App-Token", _appToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                var tickets = new List<TicketResponse>();

                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    // Response is an array of tickets
                    foreach (var ticketElement in jsonElement.EnumerateArray())
                    {
                        var ticketJson = ticketElement.GetRawText();
                        var ticket = JsonSerializer.Deserialize<TicketResponse>(ticketJson, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                            PropertyNameCaseInsensitive = true
                        });
                        if (ticket != null)
                            tickets.Add(ticket);
                    }
                }
                else if (jsonElement.TryGetProperty("data", out var dataProperty) && dataProperty.ValueKind == JsonValueKind.Array)
                {
                    // Response has a "data" property containing the array
                    foreach (var ticketElement in dataProperty.EnumerateArray())
                    {
                        var ticketJson = ticketElement.GetRawText();
                        var ticket = JsonSerializer.Deserialize<TicketResponse>(ticketJson, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                            PropertyNameCaseInsensitive = true
                        });
                        if (ticket != null)
                            tickets.Add(ticket);
                    }
                }

                return tickets;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse GLPI tickets response: {json}", ex);
            }
        }

        // 🔧 Solve Ticket (Status = 5)
        public async Task<TicketResponse> SolveTicketAsync(int ticketId)
        {
            if (_sessionToken == null)
                await LoginAsync();

            var payload = new
            {
                input = new
                {
                    status = 5, // Solved status
                    solvedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_glpiApiUrl}/Ticket/{ticketId}")
            {
                Content = content
            };
            request.Headers.Add("Session-Token", _sessionToken);
            request.Headers.Add("App-Token", _appToken);

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"GLPI API error: {response.StatusCode} - {json}");
            }

            // Return a basic response instead of fetching the ticket
            return new TicketResponse
            {
                Id = ticketId,
                Status = 5, // Solved status
              
                Solvedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        // 🔒 Close Ticket (Status = 6)
        public async Task<TicketResponse> CloseTicketAsync(int ticketId)
        {
            if (_sessionToken == null)
                await LoginAsync();

            var payload = new
            {
                input = new
                {
                    status = 6, // Closed status
                    closedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_glpiApiUrl}/Ticket/{ticketId}")
            {
                Content = content
            };
            request.Headers.Add("Session-Token", _sessionToken);
            request.Headers.Add("App-Token", _appToken);

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"GLPI API error: {response.StatusCode} - {json}");
            }

            // Return a basic response instead of fetching the ticket
            return new TicketResponse
            {
                Id = ticketId,
                Status = 6, // Closed status
              
                Closedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}