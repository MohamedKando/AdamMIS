




using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



var connectionstring = builder.Configuration.GetConnectionString("MyConnection") ??
    throw new InvalidOperationException(" Connection string 'MyConnection' Not Found.");

// Add EF Core with Identity support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionstring, sqlOptions =>
    {
        sqlOptions.CommandTimeout(90); // default is 30
       // sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    })
    .LogTo(Console.WriteLine, LogLevel.Information));



builder.Services.AddDependency(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});
// This builder use for generate logging file
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
}
);
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
//app.MapIdentityApi<ApplicationUser>();
app.MapControllers();
//app.UseExceptionHandler();
app.Run();
