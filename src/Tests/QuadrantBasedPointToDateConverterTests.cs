using System;
using NUnit.Framework;

namespace GeometryAndExchangeRate.Service.Tests;

public class QuadrantBasedPointToDateConverterTests {
    static QuadrantBasedPointToDateConverter GetConverter(float radius = 1f) => new QuadrantBasedPointToDateConverter(radius);
    [Test]
    public void FirstQuadrantIsToday() {
        Assert.That(GetConverter().Convert(.5f, .5f), Is.EqualTo(DateTime.Today));
    }
    [Test]
    public void SecondQuadrantIsYesterday() {
        Assert.That(GetConverter().Convert(-.5f, .5f), Is.EqualTo(DateTime.Today.AddDays(-1)));
    }
    [Test]
    public void ThirdQuadrantIsDayBeforeYesterday() {
        Assert.That(GetConverter().Convert(-.5f, -.5f), Is.EqualTo(DateTime.Today.AddDays(-2)));
    }
    [Test]
    public void FourthQuadrantIsTomorrow() {
        Assert.That(GetConverter().Convert(.5f, -.5f), Is.EqualTo(DateTime.Today.AddDays(1)));
    }
    [Test]
    public void CannotIdentifyDateAtAxisOrOrigin() {
        Assert.That(() => GetConverter().Convert(0f, 0f), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    [Test]
    public void CannotIdentifyDateOutsideOfCircle() {
        Assert.That(() => GetConverter().Convert(1.1f, 1.1f), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    [Test]
    public void CannotIdentifyDateOnCircle() {
        Assert.That(() => GetConverter().Convert(1f, 1f), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}