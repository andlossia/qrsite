using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Define CORS policy names
const string DevCorsPolicyName = "AllowAngular";
const string ProdCorsPolicyName = "ProdPolicy";

// Configure CORS policies
builder.Services.AddCors(options =>
{
    // Development CORS policy
    options.AddPolicy(DevCorsPolicyName, policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200") 
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });

    // Production CORS policy
    options.AddPolicy(ProdCorsPolicyName, policyBuilder =>
    {
        policyBuilder.WithOrigins("https://your-production-domain.com")
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Configure FormOptions
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 24 * 1024 * 1024; // 24 MB
});

// JWT Authentication Configuration
var jwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
var jwtIssuer = jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is missing");
var jwtAudience = jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience is missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddSingleton(new JwtService(jwtKey, jwtIssuer, jwtAudience, TimeSpan.FromHours(1)));

// Add scoped and transient services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LoggingService>();

// Add essential services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "QR Events API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Configure Canva API
builder.Services.Configure<CanvaApiConfig>(builder.Configuration.GetSection("CanvaApi"));

var app = builder.Build();

// Database migration handling for development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QR Events API V1");
        c.RoutePrefix = "swagger"; // Adjust this as needed
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "QR Events API V1");
    c.RoutePrefix = "swagger";

});


// Middleware configuration
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(app.Environment.IsDevelopment() ? DevCorsPolicyName : ProdCorsPolicyName);
app.UseAuthentication(); 
app.UseAuthorization();
app.UseErrorHandlingMiddleware();

// Map controllers
app.MapControllers();

app.Run();
