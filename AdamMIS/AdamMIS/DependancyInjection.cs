using AdamMIS.Abstractions.LoggingAbstractions;
using AdamMIS.Authentications;
using AdamMIS.Authentications.Filters;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.LogServices;
using AdamMIS.Services.ReportsServices;
using AdamMIS.Services.RolesServices;
using AdamMIS.Services.UsersServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdamMIS
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddSwaggerServices()
                    .AddMapsterConfg()
                    .AddFluentValidationConfg()
                    .AddAuthConfg(configuration);
                   
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<ILoggingContext, LoggingContext>();
            services.AddScoped<AuditSaveChangesInterceptor>();
            services.AddScoped<PermissionsSeeder>();



            services.AddHttpContextAccessor();
            services.AddProblemDetails();
            return services;
        }

        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
        private static IServiceCollection AddMapsterConfg(this IServiceCollection services)
        {
            // to make mapping config seen in all program
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }
        private static IServiceCollection AddFluentValidationConfg(this IServiceCollection services)
        {
            //to make fluent validation seen in all program

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
            return services;
        }
        private static IServiceCollection AddAuthConfg(this IServiceCollection services, IConfiguration configuration)
        {
            //to make Identity Service seen in all program
             services.AddSingleton<IJwtProvider, JwtProvider>();
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            // this line to bind the jwt setting from appsetting with jwtoption class and 
            // the difference between it and the previuse line is the second line validate the data using dataannotation
            //services.AddOptions<JwtOptions>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();
            var JwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders(); 

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.Key!)),
                    ValidAudience = JwtSettings!.Audience,
                    ValidIssuer = JwtSettings.Issuer


                };
            });
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;


                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;


            });
            return services;
        }
    }
}
