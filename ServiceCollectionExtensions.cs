using Infrastructure.Configuration;
using Infrastructure.Core.Entities;
using Infrastructure.Core.Interfaces.Application.EntityServices;
using Infrastructure.Core.Interfaces.Application.UtilityServices;
using Infrastructure.Core.Interfaces.Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IT_Automation.API.Application.EntityServices;
using IT_Automation.API.Application.UtilityServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace IT_Automation.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(ProjectConfig.Instance.ConnectionString));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            // Register MemoryCache
            services.AddMemoryCache();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1); // صلاحية التوكين هي ساعة واحدة
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = ProjectConfig.Instance.JwtSettings.Issuer,
                        ValidAudience = ProjectConfig.Instance.JwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ProjectConfig.Instance.JwtSettings.Secret))
                    };
                });

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //AddServices
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IVerificationService, VerificationService>();
            //services.AddScoped<IFileService, FileService>();
            services.AddHttpContextAccessor();

            return services;
        }
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Controllers and enable API Explorer
            services.AddControllers(); // Basic controller services
            services.AddEndpointsApiExplorer(); // Enables discovering API endpoints for Swagger/OpenAPI

            // API Versioning Setup
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version
                options.AssumeDefaultVersionWhenUnspecified = true; // Use default if version isn't specified
                options.ReportApiVersions = true; // Return API version information in response headers
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version"); // Read API version from request headers
            });

            // Swagger Setup
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddSwaggerGen(option =>
            {
                // Define different API docs for various apps
                option.SwaggerDoc("DashboardApp", new OpenApiInfo { Title = "Dashboard App API", Version = "1.0" });
                option.SwaggerDoc("UserApp", new OpenApiInfo { Title = "User App API", Version = "2.0" });

                option.IncludeXmlComments(xmlPath); // Include XML comments for Swagger UI

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                //// Require both API Key and Bearer Token for all endpoints
                //option.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        new string[] {}
                //    }
                //});
            });

            //// Dynamic Authorization Policies
            //var authorizationPolicies = configuration.GetSection("AuthorizationPolicies").Get<Dictionary<string, Dictionary<string, object>>>();
            //services.AddAuthorization(options =>
            //{
            //    foreach (var policy in authorizationPolicies)
            //    {
            //        var policyName = policy.Key;
            //        var conditions = policy.Value;

            //        options.AddPolicy(policyName, builder =>
            //            builder.Requirements.Add(new DynamicAuthorizationRequirement(policyName, conditions)));
            //    }
            //});

            //services.AddSingleton<IAuthorizationHandler, DynamicAuthorizationHandler>(); // Register the dynamic authorization handler
            // Configure file upload size limits for multipart forms
            //services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;  // 10 MB file size limit
            //});

            // Enable ProblemDetails for standardized error responses
            services.AddProblemDetails();

            // Lowercase URLs for route mapping
            services.AddRouting(options => options.LowercaseUrls = true);

            // Configure JSON options for the controllers
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Handle enums as strings in JSON
            });

            return services;
        }

    }

}
