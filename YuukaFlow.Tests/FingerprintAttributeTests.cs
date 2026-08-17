using YuukaFlow.Core;

namespace YuukaFlow.Tests;

public class FingerprintAttributeTests
{
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("0000000a", 10)]
    [InlineData("7fffffff", int.MaxValue)]
    [InlineData("80000000", int.MinValue)]
    [InlineData("ffffffff", -1)]
    public void Constructor_ParsesHexAsTwosComplementFingerprint(string hex, int expectedCode)
    {
        var attribute = new FingerprintAttribute(hex);

        Assert.Equal(new Fingerprint(expectedCode), attribute.Fingerprint);
    }

    [Fact]
    public void Fingerprint_RoundTripsThroughFingerprintToStringFormat()
    {
        var fingerprint = Fingerprint.From("some-flowchart-content");
        var hex = $"{fingerprint.Code:x8}";

        var attribute = new FingerprintAttribute(hex);

        Assert.Equal(fingerprint, attribute.Fingerprint);
    }
}
