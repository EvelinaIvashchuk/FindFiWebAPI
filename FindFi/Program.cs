using FindFi.Bll;
using FindFi.Dal;
using FindFi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Allow env var overrides (e.g., ConnectionStrings__Default)
builder.Configuration.AddEnvironmentVariables();

// Logging (Microsoft logger)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ProblemDetails + Swagger/OpenAPI
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DAL + BLL registration
var connectionString = builder.Configuration.GetConnectionString("DB1")
                      ?? Environment.GetEnvironmentVariable("ConnectionStrings__DB1")
                      ?? string.Empty;
builder.Services.AddDal(connectionString);
builder.Services.AddBll();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();