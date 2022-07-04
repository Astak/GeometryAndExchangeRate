using System.Globalization;

namespace GeometryAndExchangeRate.Service;
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