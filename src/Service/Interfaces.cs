using GeometryAndExchangeRate.Service.Models;
namespace GeometryAndExchangeRate.Service;

public interface IPointToDateConverter {
    DateTime Convert(float x, float y);
}

public interface IExchangeRateService {
    Task<ExchangeRate> GetExchangeRateAsync(DateTime onDate);
}