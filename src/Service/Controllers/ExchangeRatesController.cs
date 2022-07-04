using Microsoft.AspNetCore.Mvc;
using GeometryAndExchangeRate.Service.Models;

namespace GeometryAndExchangeRate.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExchangeRatesController : ControllerBase {
    readonly ILogger<ExchangeRatesController> logger;
    readonly IPointToDateConverter pointToDateConverter;
    readonly IExchangeRateService exchangeRateService;

    public ExchangeRatesController(ILogger<ExchangeRatesController> logger, IPointToDateConverter pointToDateConverter, IExchangeRateService exchangeRateService) {
        this.logger = logger;
        this.pointToDateConverter = pointToDateConverter;
        this.exchangeRateService = exchangeRateService;
    }

    [HttpGet("x{x:float}y{y:float}")]
    public async Task<ExchangeRate> Get(float x, float y) {
        DateTime onDate = pointToDateConverter.Convert(x, y);
        return await exchangeRateService.GetExchangeRateAsync(onDate);
    }
}
