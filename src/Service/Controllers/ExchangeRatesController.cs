using Microsoft.AspNetCore.Mvc;
using GeometryAndExchangeRate.Service.Models;
using GeometryAndExchangeRate.Service.ErrorHandling;

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
        DateTime onDate;
        try{
            onDate = pointToDateConverter.Convert(x, y);
        } catch(ArgumentOutOfRangeException ex) {
            throw new UserFriendlyException("Argument out of range", ex.Message, ex);
        }
        try {
            return await exchangeRateService.GetExchangeRateAsync(onDate);
        } catch(ExchangeRateServiceException ex) {
            throw new UserFriendlyException("No exchane rate data", ex.Message, ex);
        }
    }
}
