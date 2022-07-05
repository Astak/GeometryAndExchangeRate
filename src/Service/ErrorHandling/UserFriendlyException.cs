namespace GeometryAndExchangeRate.Service.ErrorHandling;

public class UserFriendlyException : Exception {
    public UserFriendlyException(string title, string detail, Exception? inner) : base(title, inner) {
        this.ErrorData = new {
            Title = title,
            Detail = detail
        };
    }
    public object ErrorData { get; private set; }
}