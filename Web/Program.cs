using Core;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using DAL;
using Data;
using Data.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Web.Filters;
using Web.Services;

Log.Logger = (Serilog.ILogger)new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
    //.MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//logger
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
.AddFluentValidation(options =>
{
    // Validate child properties and root collection elements
    options.ImplicitlyValidateChildProperties = true;
    options.ImplicitlyValidateRootCollectionElements = true;
    // Automatic registration of validators in assembly
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

//Identity
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddIdentity<Account, IdentityRole>(options => options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
       o.TokenLifespan = TimeSpan.FromMinutes(10));

//DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
               b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

//Identity Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(b =>
{
    b.SaveToken = true;
    b.RequireHttpsMetadata = false;
    b.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        // ValidateIssuerSigningKey = true,
        ValidateLifetime = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))

    };
});
builder.Services.AddAuthorization();
//services
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()//.WithOrigins("https://localhost:8081") // note the port is included 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<IEmailService, EmailService>();
//Localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
         new CultureInfo("ar"),
         new CultureInfo("en"),
    };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: "ar", uiCulture: "ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context => 
    //{
    //    var languages = context.Request.Headers["Accept-Language"].ToString();
    //    var currentLanguage = languages.Split(',').FirstOrDefault();
    //    var defaultLanguage = string.IsNullOrEmpty(currentLanguage) ? "ar" : currentLanguage;

    //    if (defaultLanguage != "ar" && defaultLanguage != "en")
    //    {
    //        defaultLanguage = "ar";
    //    }
    //    return Task.FromResult(new ProviderCultureResult(defaultLanguage, defaultLanguage));
    //}));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//versioning
builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware(typeof(ErrorHandlingMiddleWare));

app.UseRouting();

//localization should be after UseRouting();
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

app.UseCors("MyAllowedOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
