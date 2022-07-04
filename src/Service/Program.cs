using GeometryAndExchangeRate.Service;
using GeometryAndExchangeRate.Service.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
});

builder.Services.AddSingleton<IPointToDateConverter>(new QuadrantBasedPointToDateConverter(1f));
builder.Services.AddSingleton<IExchangeRateService>(new DwsClient());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GeometryAndExchangeRate Service");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
