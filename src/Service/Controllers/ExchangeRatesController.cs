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
        logger.LogInformation("{timestamp:G}: The exchange rate is requested for {point}",DateTime.UtcNow, (x:x, y:y));
        DateTime onDate;
        try{
            onDate = pointToDateConverter.Convert(x, y);
            logger.LogInformation("{timestamp:G}: The {point} is converted to {onDate:yyyyMMdd}", DateTime.UtcNow, (x:x, y:y), onDate);
        } catch(ArgumentOutOfRangeException ex) {
            logger.LogError(ex, "{timestamp:G}: Could not convert {point} to the date.\r\n", DateTime.UtcNow, (x:x, y:y));
            throw new UserFriendlyException("Argument out of range", ex.Message, ex);
        }
        try {
            return await exchangeRateService.GetExchangeRateAsync(onDate).ConfigureAwait(false);
        } catch(ExchangeRateServiceException ex) {
            logger.LogError(ex, "{timestamp:G}: Could not retrieve exchange rate data for {date:yyyyMMdd}.\r\n", DateTime.UtcNow, onDate);
            throw new UserFriendlyException("No exchane rate data", ex.Message, ex);
        }
    }
}
