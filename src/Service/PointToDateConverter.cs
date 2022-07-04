namespace GeometryAndExchangeRate.Service {
    using System.Globalization;
    public class QuadrantBasedPointToDateConverter : IPointToDateConverter {
        readonly float radius;
        public QuadrantBasedPointToDateConverter(float radius) {
            this.radius = radius;
        }
        public DateTime Convert(float x, float y) {
            var circleRelatedPosition = GeometryCalculator.CalcCircleRelatedPosition(radius, x, y);
            if(circleRelatedPosition != CircleRelatedPosition.Inside) {
                throw new ArgumentOutOfRangeException(string.Empty, string.Format(CultureInfo.CurrentUICulture, "Could not identify the date, because the distance to the center is not less than {0}.", radius));
            }
            var quadrant = GeometryCalculator.IdentifyQuadrant(x, y);
            switch(quadrant) {
                case Quadrant.First:
                    return DateTime.Today;
                case Quadrant.Second:
                    return DateTime.Today.AddDays(-1);
                case Quadrant.Third:
                    return DateTime.Today.AddDays(-2);
                case Quadrant.Fourth:
                    return DateTime.Today.AddDays(1);
                default:
                    throw new ArgumentOutOfRangeException(string.Empty, "Could not identify the date, because x or y is zero.");
            }
        }
    }
}
namespace Microsoft.Extensions.DependencyInjection {
    using Microsoft.Extensions.Options;
    using GeometryAndExchangeRate.Service;
    static class QuadrantBasedPointToDateConverterExtensions {
        public static IServiceCollection AddQuadrantBasedPointToDateConverter(this IServiceCollection services, IConfiguration configuration) {
            services.AddSingleton<IPointToDateConverter>(serviceProvider => {
                var options = serviceProvider.GetRequiredService<IOptions<QuadrantBasedPointToDateConverterOptions>>();
                return new QuadrantBasedPointToDateConverter(options.Value.CircleRadius);
            });
            services.Configure<QuadrantBasedPointToDateConverterOptions>(configuration.GetRequiredSection(QuadrantBasedPointToDateConverterOptions.Key));
            return services;
        }
    }
    class QuadrantBasedPointToDateConverterOptions {
        public const string Key = "QuadrantBasedPointToDateConverter";
        public float CircleRadius { get; set; }
    }
}