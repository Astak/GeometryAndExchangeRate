using Microsoft.AspNetCore.Mvc;

namespace GeometryAndExchangeRate.Service.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class DefaultController : ControllerBase {
    [Route("/")]
    [Route("/index.html")]
    [Route("/docs")]
    [Route("/swagger")]
    public IActionResult Get() => new RedirectResult("~/swagger");
}