using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models.Identity;
using SellersZone.Infra;
using SellersZone.Infra.Helpers;
using SellersZone.Infra.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

#region StoreContext...
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = false;

    // Token settings
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

}).AddEntityFrameworkStores<StoreContext>()
  .AddDefaultTokenProviders()
  .AddSignInManager<SignInManager<AppUser>>();
#endregion

#region Auth...
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{   // Set token expiration to 3 days
    options.ValidationInterval = TimeSpan.FromDays(3);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // string validIssuer = builder.Configuration.GetValue<string>("Token:Issuer") ?? string.Empty;
    // string issuerSigningKey = builder.Configuration.GetValue<string>("Token:Key") ?? string.Empty;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidateIssuer = false,
        ValidateAudience = false,
    };

});

builder.Services.AddAuthorization();

#endregion

#region Configure Bad Request

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    //ActionContext => can get the model state errors
//    options.InvalidModelStateResponseFactory = ActionContext =>
//    {
//        var errors = ActionContext.ModelState
//                   .Where(e => e.Value?.Errors.Count > 0)
//                   .SelectMany(x => x.Value.Errors)
//                   .Select(x => x.ErrorMessage).ToArray();
//        var errorResponse = new ApiValidationError
//        {
//            ValidationErrors = errors
//        };

//        return new BadRequestObjectResult(errorResponse);
//    };
//});
#endregion

#region Services Collection
builder.Services.AddScoped<IFileStorageService, StorageService>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishListRepository, WishListRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICarouselsRepository, CarouselsRepository>();
builder.Services.AddScoped<IPurchaseProductRepository, PurchaseProductRepository>();
builder.Services.AddScoped<ISideEarningRepository, SideEarningRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();
#endregion

#region Cors...
builder.Services.AddCors(options =>
{
    string localFrontendUrl = builder.Configuration.GetValue<string>("localFrontendUrl") ?? string.Empty;
    string localBackendUrl = builder.Configuration.GetValue<string>("localBackendUrl") ?? string.Empty;
    string prodDashboardFrontendUrl = builder.Configuration.GetValue<string>("prodDashboardFrontendUrl") ?? string.Empty;
    string prodClientFrontendUrl = builder.Configuration.GetValue<string>("prodClientFrontendUrl") ?? string.Empty;

    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins(localFrontendUrl, prodDashboardFrontendUrl, prodClientFrontendUrl, localBackendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

builder.Services.AddDataProtection()
               .PersistKeysToFileSystem(new DirectoryInfo(@"C:\SellersZone\SellersZone.Server\DataProtection"))
               .SetDefaultKeyLifetime(TimeSpan.FromDays(7));

#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.WebHost.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot/projects";
});

#region Swagger...
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    //for jwt
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Auth Bearer Sheme",
        Name = "Authorisation",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securitySchema, new [] {"Bearer"}
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});
#endregion

#region Hangfire...
// Add Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();
#endregion

// For HangFire
builder.Services.AddScoped<WalletRepository>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Global middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseCors("AllowSpecificOrigin");
app.UseStatusCodePagesWithReExecute("/errors/{0}"); // `{0}` represents the status code

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.MapControllers();

//await SeedDatabase();

app.UseHangfireDashboard();
app.UseHangfireServer();

#region Methods
async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await IdentityConfiguration.SeedIdentityAsync(userManager, roleManager);
    }
}
#endregion

//Serve admin app for paths starting with /admin
app.MapWhen(context => context.Request.Path.StartsWithSegments("/admin"), adminApp =>
{
    adminApp.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", "admin"))
    });

    adminApp.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        var indexPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", "admin", "index.html");
        await context.Response.SendFileAsync(indexPath);
    });
});

// Serve user app for all other paths
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/admin"), userApp =>
{
    userApp.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", "user"))
    });

    userApp.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        var indexPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "projects", "user", "index.html");
        await context.Response.SendFileAsync(indexPath);
    });
});



app.Run();

