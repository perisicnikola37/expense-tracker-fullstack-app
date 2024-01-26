using Domain.Interfaces;
using Domain.Models;
using Domain.Validators;
using ExpenseTrackerApi.conf;
using ExpenseTrackerApi.Handlers;
using ExpenseTrackerApi.Middlewares;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Persistence;
using Service;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// disabled
// builder.Services.AddHttpLogging(o => { });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.WithOrigins("https://example.com"); });
});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

// Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BlogOwnerPolicy", policy => { policy.Requirements.Add(new BlogAuthorizationRequirement()); });
    options.AddPolicy("ExpenseOwnerPolicy",
        policy => { policy.Requirements.Add(new ExpenseAuthorizationRequirement()); });
    options.AddPolicy("IncomeOwnerPolicy",
        policy => { policy.Requirements.Add(new IncomeAuthorizationRequirement()); });
});

builder.Services.AddScoped<IAuthorizationHandler, BlogAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ExpenseAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IncomeAuthorizationHandler>();

var connectionString = configuration["DefaultConnection"];
if (connectionString == null) throw new ArgumentNullException(nameof(connectionString), "DefaultConnection is null");

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 35)),
        b => b.MigrationsAssembly("ExpenseTrackerApi")
    );
});

builder.Services.AddAuthentication();

// important for adding routes based on controllers
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// validators
builder.Services.AddScoped<IValidator<Blog>, BlogValidator>();
builder.Services.AddScoped<IValidator<ExpenseGroup>, ExpenseGroupValidator>();
builder.Services.AddScoped<IValidator<Expense>, ExpenseValidator>();
builder.Services.AddScoped<IValidator<IncomeGroup>, IncomeGroupValidator>();
builder.Services.AddScoped<IValidator<Income>, IncomeValidator>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<Reminder>, ReminderValidator>();

// services
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IGetAuthenticatedUserIdService, GetAuthenticatedUserIdService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<GetCurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GetAuthenticatedUserIdService>();
builder.Services.AddScoped<ReminderService>();
builder.Services.AddScoped<ExpenseGroupService>();
builder.Services.AddScoped<IncomeGroupService>();

// repositories
builder.Services.AddScoped<ReminderRepository>();

// Jwt configuration
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.ConfigureSwaggerGen();

// Rate Limiter configuration
builder.Services.ConfigureRateLimiter();

var app = builder.Build();

// app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// auth
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

// this middleware needs to be after .net auth middlewares!
app.UseMiddleware<ClaimsMiddleware>();
app.UseGlobalExceptionHandler();

app.UseCors();

app.Run();