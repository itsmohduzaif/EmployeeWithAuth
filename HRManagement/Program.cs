using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Entities;
using HRManagement.ExceptionHandlers;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.Services;
using HRManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Will upgrade the whole project later to follow this ApiBehaviour (need to deete existing modelstate)
// th ehttps://www.perplexity.ai/search/using-hrmanagement-dtos-using-gTBta5m1Q9GFRLoSOlX7xA
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.InvalidModelStateResponseFactory = context =>
//    {
//        var errors = context.ModelState.Values
//            .SelectMany(v => v.Errors)
//            .Select(e => e.ErrorMessage)
//            .ToList();

//        var response = new ApiResponse(false, "Body Validation failed", 400, errors);
//        return new BadRequestObjectResult(response);
//    };
//});






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



builder.Services.AddSingleton<JwtHandler>();
builder.Services.AddSingleton<BlobStorageService>();






// Configure CORS to allow Angular app on localhost:4200
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});








var app = builder.Build();

// For exception handling 
app.UseExceptionHandler();

// Enable CORS for frontend
app.UseCors("AllowAngularApp");



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
