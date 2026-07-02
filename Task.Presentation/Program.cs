using Task.Application.Interfaces.Services;
using Task.Application.Services;
using Task.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

// Register Application Services
builder.Services.AddScoped<IMissionTemplateService, MissionTemplateService>();
builder.Services.AddScoped<IMissionTemplateService, MissionTemplateService>();
builder.Services.AddScoped<IUserMissionService, UserMissionService>();
builder.Services.AddScoped<IMissionSubmissionService, MissionSubmissionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();