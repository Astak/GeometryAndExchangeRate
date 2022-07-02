namespace GeometryAndExchangeRate.Service;

static class GeometryCalculator {
    public static Quadrant IdentifyQuadrant(float x, float y) {
        if(x > 0f) {
            if(y > 0) return Quadrant.First;
            if(y < 0) return Quadrant.Fourth;
        } else {
            if(y > 0) return Quadrant.Second;
            if(y < 0) return Quadrant.Third;
        }
        return Quadrant.None;
    }
    public static CircleRelatedPosition CalcCircleRelatedPosition(float radius, float x, float y) {
        float squaredCenterDistance = x * x + y * y;
        float squaredRadius = radius * radius;
        float diff = squaredRadius - squaredCenterDistance;
        Console.WriteLine($"{squaredCenterDistance}, {squaredRadius}, {diff}");
        if(diff > 0) {
            return CircleRelatedPosition.Inside;
        } else if(diff < 0) {
            return CircleRelatedPosition.Outside;
        } else {
            return CircleRelatedPosition.On;
        }
    }
}
public enum Quadrant { None, First, Second, Third, Fourth }
public enum CircleRelatedPosition { Inside, On, Outside }