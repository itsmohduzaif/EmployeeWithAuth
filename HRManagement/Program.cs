using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Entities;
using HRManagement.ExceptionHandlers;
using HRManagement.Helpers;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.SeedConfiguration;
using HRManagement.Services.Accounts;
using HRManagement.Services.Drafts;
using HRManagement.Services.Emails;
using HRManagement.Services.Employees;
using HRManagement.Services.EmployeesExcel;
using HRManagement.Services.LeaveRequests;
using HRManagement.Services.LeaveTypes;
using HRManagement.Services.Notifications;
using HRManagement.Services.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});

// Will upgrade the whole project later to follow this ApiBehaviour (need to deete existing modelstate)
// th ehttps://www.perplexity.ai/search/using-hrmanagement-dtos-using-gTBta5m1Q9GFRLoSOlX7xA
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = new ApiResponse(false, "Body Validation failed", 400, errors);
        return new BadRequestObjectResult(response);
    };
});
// The required property will also not allow null or empty strings in the request body and give the error response





// Register exception handling services
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();




builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IDraftService, DraftService>();

builder.Services.AddSingleton<JwtHandler>();
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddHostedService<ExpiryNotificationService>();
builder.Services.AddSingleton<EmployeeExcelExporter>();
builder.Services.AddSingleton<EmployeeExcelImporter>();
builder.Services.AddScoped<IEmployeeExcel, EmployeeExcel>();

builder.Services.AddTransient<LeaveRequestHelper>();






builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>(opt =>
{
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequiredUniqueChars = 4;
})
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();   // Adding this for token genration in forgot password

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
builder.Services.AddAuthentication(options =>
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
            ValidIssuer = jwtSettings["ValidIssuer"],
            ValidAudience = jwtSettings["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("SecretKey").Value))
        };

        // Custom error events adding for custom response of 401 and 403 status codes
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                var apiResponse = new ApiResponse(false, "Unauthorized", 401, null);
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(apiResponse);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var apiResponse = new ApiResponse(false, "Forbidden", 403, null);
                return context.Response.WriteAsJsonAsync(apiResponse);
            }
        };

    });





builder.Services.AddAutoMapper(typeof(MappingProfile));


// Configure CORS to allow Angular app on localhost:4200
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});




var app = builder.Build();

// Seed data for Users Table if the database is empty.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    await DbInitializer.SeedAsync(context, userManager, roleManager);
}






// For exception handling 
app.UseExceptionHandler();

// Handle 415 Unsupported Media Type with custom response - as when no dto is being sent in the request body it gives an error with format other than ApiResponse
app.Use(async (context, next) =>
{
    // Only intercept for POST/PUT/PATCH with a body
    if (HttpMethods.IsPost(context.Request.Method) ||
        HttpMethods.IsPut(context.Request.Method) ||
        HttpMethods.IsPatch(context.Request.Method))
    {
        var contentType = context.Request.ContentType;

        // If Content-Type is missing or not application/json
        if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse(false, "Unsupported Media Type. Please set Content-Type to application/json.", 415, null);
            await context.Response.WriteAsJsonAsync(response);
            return; // short-circuit
        }
    }

    await next(); // continue to next middleware if content type is OK
});





// Enable CORS for frontend
app.UseCors("AllowAngularApp");




// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
