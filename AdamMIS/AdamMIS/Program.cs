




using AdamMIS.Abstractions.LoggingAbstractions;
using AdamMIS.Authentications.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Serilog;

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
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// This builder use for generate logging file
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
}
);


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
//    FileProvider = new PhysicalFileProvider(@"\\192.168.1.203\e$\App-data"),
//    RequestPath = ""  // Empty means serve from root
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseStaticFiles(); // Default wwwroot (if you have one)

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"\\192.168.1.203\e$\App-data\user-photos"),
    RequestPath = "/user-photos" // Map to /user-photos path
});
//app.UseDeveloperExceptionPage();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseForwardedHeaders();
//app.MapIdentityApi<ApplicationUser>();
app.MapControllers();
//app.UseExceptionHandler();
app.Run();
