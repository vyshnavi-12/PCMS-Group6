using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PCMS_Backend.Data;
using PCMS_Backend.Hubs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Repositories;
using PCMS_Backend.Services;
using PCMS_Backend.Services.Scheduling.Builders;
using PCMS_Backend.Services.Scheduling.Engines;
using PCMS_Backend.Services.Scheduling.Interfaces;
using PCMS_Backend.Services.Scheduling.Repositories;
using PCMS_Backend.Services.Scheduling.Rules;
using PCMS_Backend.Services.Scheduling.Scoring;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPhysicianService, PhysicianService>();
builder.Services.AddScoped<IPhysicianRepository, PhysicianRepository>();
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();
builder.Services.AddScoped<ISupervisorService, SupervisorService>();
builder.Services.AddScoped<ICoverageAssignmentsService, CoverageAssignmentsService>();
builder.Services.AddScoped<ICoverageAssignmentsRepository, CoverageAssignmentsRepo>();

// Controllers
builder.Services.AddControllers();

builder.Services.AddDbContext<PcmsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IPhysicianRepository, PhysicianRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMyScheduleRepository, MyScheduleRepository>();
builder.Services.AddScoped<IMyScheduleService, MyScheduleService>();
builder.Services.AddScoped<ISwapRequestRepository, SwapRequestRepository>();
builder.Services.AddScoped<ISwapRequestService, SwapRequestService>();
builder.Services.AddScoped<ICoverageScheduleRepository, CoverageScheduleRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICoverageScheduleService, CoverageScheduleService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();



// ===========================
// Recommendation Engine
// ===========================

builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

builder.Services.AddScoped<IRecommendationContextBuilder, RecommendationContextBuilder>();

builder.Services.AddScoped<IPhysicianRecommendationService, PhysicianRecommendationService>();

builder.Services.AddScoped<IFairnessScorer, FairnessScorer>();

// ===========================
// Recommendation Rules
// ===========================

builder.Services.AddScoped<IEligibilityRule, SpecialtyEligibilityRule>();

builder.Services.AddScoped<IEligibilityRule, LeaveEligibilityRule>();

builder.Services.AddScoped<IEligibilityRule, RestGapEligibilityRule>();

builder.Services.AddScoped<IEligibilityRule, WeeklyLimitEligibilityRule>();

builder.Services.AddScoped<IEligibilityRule, ExternalShiftEligibilityRule>();


builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vue frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!)
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<ScheduleHub>("/scheduleHub");
app.MapHub<SwapRequestHub>("/swapRequests");
app.MapHub<UnavailableRequestHub>("/unavailableRequestHub");

app.Run();
