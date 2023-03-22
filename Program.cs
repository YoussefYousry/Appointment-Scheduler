using AppointmentScheduler.Extensions;
using BusinessLayer_AppointmentScheduler.Services;
using DataLayer_AppointmentScheduler.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using NLog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureSQLContext(builder.Configuration); // Call extension method for DBContext
builder.Services.ConfigureCors(); // call extension method for CORS
builder.Services.ConfigureIISIntegration(); // call extension method for IIS integration
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config.xml")); // using Logger
builder.Services.ConfigureLifeTimeServices(); // calling extension method for logger 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // AUTO MAPPER
builder.Services.ConfigureIdentity<User>(); // call extension method for Identity
builder.Services.ConfigureIdentity<Employee>(); // call extension method for Identity
builder.Services.ConfigureIdentity<Manager>(); // call extension method for Identity
builder.Services.ConfigureIdentity<Admin>(); // call extension method for Identity
builder.Services.AddAuthentication(); // Adding authentication
builder.Services.ConfigureJWT(builder.Configuration);//call extension method for JWT


builder.Services.AddControllers().AddJsonOptions(
  opt =>
      opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles // To Ignore Cycles
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger); // Global error handler Extension Method

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   // app.ConfigureExceptionHandler(Logger);
}

app.UseCors("CorsPolicy"); // call extension method for CORS in the (Middleware)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All //ALLOWING HEADERS (CORS)

});

app.UseHttpsRedirection();

app.UseAuthentication(); // Identity

app.UseAuthorization();

app.MapControllers();

app.Run();
