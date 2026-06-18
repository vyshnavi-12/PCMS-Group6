using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Repositories;
using PCMS_Backend.Repositories.Interfaces;
using PCMS_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<PcmsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<
    ICoverageScheduleRepository,
    CoverageScheduleRepository>();

builder.Services.AddScoped<
    ICoverageScheduleService,
    CoverageScheduleService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();