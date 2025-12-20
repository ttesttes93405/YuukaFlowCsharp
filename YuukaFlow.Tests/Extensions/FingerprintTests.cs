namespace YuukaFlow.Tests.Extensions;

using YuukaFlow.Core;
using static YuukaFlow.Core.Extensions.FingerprintExtensions;

public class FingerprintTests
{

    [Theory]
    [InlineData(null, 0)]
    [InlineData(6, 6)]
    [InlineData("hello", 242308077)]
    public void GetObjectFingerprint_ShouldMatchExpectedValue(object? obj, int expectedValue)
    {
        var expected = new Fingerprint(expectedValue);
        var result = Fingerprint.From(obj);
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(new[] { 123, 456, 789 }, 222501)]
    [InlineData(new int[0], 0)]
    public void CombineFingerprint_ShouldMatchExpectedValue(int[] value, int expected)
    {
        var expectedFingerprint = new Fingerprint(expected);
        var resultFingerprint = CombineFingerprints(value.Select(v => new Fingerprint(v)).ToArray());
        Assert.Equal(expectedFingerprint, resultFingerprint);
    }


    [Fact]
    public void GetArrayFingerprint_WhenArrayContentIsSame_ShouldReturnStableCode()
    {
        var a1 = new[] { 1, 2, 3, 4, 5 };
        var a2 = new[] { 1, 2, 3, 4, 5 };

        var code1 = GetArrayFingerprint<int>(a1);
        var code2 = GetArrayFingerprint<int>(a2);

        Assert.Equal(code1, code2);
    }


    [Fact]
    public void GetObjectFingerprint_ForKnownObjects_ShouldMatchExpectedValues()
    {
        var result1 = Fingerprint.From("test");
        var result2 = Fingerprint.From(12345);
        var result3 = Fingerprint.From<object?>(null);

        Assert.Equal(new(7250582), result1);
        Assert.Equal(new(12345), result2);
        Assert.Equal(Fingerprint.None, result3);
    }


}
