using Microsoft.AspNetCore.Mvc;
using GeometryAndExchangeRate.Service.Models;

namespace GeometryAndExchangeRate.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExchangeRatesController : ControllerBase {
    readonly ILogger<ExchangeRatesController> _logger;

    public ExchangeRatesController(ILogger<ExchangeRatesController> logger) {
        _logger = logger;
    }

    [HttpGet("x{x:float}y{y:float}")]
    public ExchangeRate Get(float x, float y) {
        throw new NotImplementedException();
    }
}
