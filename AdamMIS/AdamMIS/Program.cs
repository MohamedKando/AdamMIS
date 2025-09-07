

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



var connectionstring = builder.Configuration.GetConnectionString("MyConnection") ??
    throw new InvalidOperationException(" Connection string 'MyConnection' Not Found.");

// Add EF Core with Identity support
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlServer(connectionstring, sqlOptions =>
    {
        sqlOptions.CommandTimeout(90); // default is 30
    })
    .LogTo(Console.WriteLine, LogLevel.Information);

    // Add logging interceptor
   // options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
});
builder.Services.AddDependency(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://192.168.1.203",
                "https://192.168.1.203",
                "http://localhost:4200",
                "https://localhost:4200",
                "http://adamhmis.adam.hospital",
                "https://adamhmis.adam.hospital"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// This builder use for generate logging file
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
}
);
builder.Services.AddSignalR();
builder.Services.AddHttpClient<ITicketingService, TicketingService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                              | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionsSeeder>();
    await permissionSeeder.SeedPermissionsAsync();
}
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.WebRootPath, "Uploads")),
//    RequestPath = "/Uploads"
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseStaticFileMappings(new Dictionary<string, string>
{
    { "/user-photos", @"\\192.168.1.203\e$\App-data\user-photos" },
    { "/CrystalReportConfig",     @"\\192.168.1.203\e$\Programs\CrystalReportConfig" },
    { "/crystal_reports",     @"\\192.168.1.203\e$\App-data\crystal_reports" }

});

//app.UseDeveloperExceptionPage();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("ApiCorsPolicy"); // ✅ must come right after UseRouting

app.UseAuthentication();      // ✅ if you use auth
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub")
             .RequireCors("ApiCorsPolicy");

    endpoints.MapControllers(); // 👈 controllers mapped here
});

app.UseForwardedHeaders();
//app.MapIdentityApi<ApplicationUser>();
//app.UseExceptionHandler();

app.Run();

