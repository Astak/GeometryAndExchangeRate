using GeometryAndExchangeRate.Service.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => {
    options.Filters.Add<HttpResponseExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
});

builder.Services.AddQuadrantBasedPointToDateConverter(builder.Configuration);
builder.Services.AddDwsClient(builder.Configuration);
builder.Services.AddLogging(loggingBuilder => {
    var loggingSection = builder.Configuration.GetRequiredSection("Logging");
    loggingBuilder.AddFile(loggingSection);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDwsClient();

app.MapControllers();

app.Run();
