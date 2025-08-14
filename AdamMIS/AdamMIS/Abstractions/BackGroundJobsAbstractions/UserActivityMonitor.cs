namespace AdamMIS.Abstractions.BackGroundJobsAbstractions
{
    public class UserActivityMonitor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public UserActivityMonitor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;

                // Assume JWT expires in 15 min from last activity
                var expiredUsers = context.acivityLogs
                    .Where(l => l.IsOnline && l.LastActivityTime.AddMinutes(2) < now)
                    .ToList();

                foreach (var user in expiredUsers)
                {
                    user.IsOnline = false;
                }

                if (expiredUsers.Any())
                {
                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }

}
