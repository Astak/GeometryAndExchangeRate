var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
});

builder.Services.AddQuadrantBasedPointToDateConverter(builder.Configuration);
builder.Services.AddDwsClient(builder.Configuration);

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

app.UseDwsClient();

app.MapControllers();

app.Run();
