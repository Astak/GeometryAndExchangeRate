using System.Globalization;

namespace GeometryAndExchangeRate.Service.ErrorHandling;

public abstract class ExchangeRateServiceException : Exception {}
public class NoExchangeRateDataOnDateException : ExchangeRateServiceException {
    readonly string onDate;
    readonly string? availableDate;
    public NoExchangeRateDataOnDateException(string onDate, string? availableDate) {
        this.onDate = onDate;
        this.availableDate = availableDate;
    }
    public override string Message => string.Format(CultureInfo.CurrentUICulture, "No exchange data is available on {0}, try {1}.", onDate, availableDate);
}

public class NoExchangeRateDataForCurrencyException : ExchangeRateServiceException {
    readonly string currencyCode;
    public NoExchangeRateDataForCurrencyException(string currencyCode) {
        this.currencyCode = currencyCode;
    }
    public override string Message => string.Format(CultureInfo.CurrentUICulture, "No exchange data is available for '{0}'.", currencyCode);
}