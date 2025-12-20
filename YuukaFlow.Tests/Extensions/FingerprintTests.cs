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
        var result = GetFingerprint(obj);
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(new[] { 123, 456, 789 }, 222501)]
    [InlineData(new int[0], 0)]
    public void CombineFingerprint_ShouldMatchExpectedValue(int[] value, int expected)
    {
        var expectedFingerprint = new Fingerprint(expected);
        var resultFingerprint = CombineFingerprint(value.Select(v => new Fingerprint(v)).ToArray());
        Assert.Equal(expectedFingerprint, resultFingerprint);
    }


    [Fact]
    public void GetArrayFingerprint_WhenArrayContentIsSame_ShouldReturnStableCode()
    {
        var a1 = new[] { 1, 2, 3, 4, 5 };
        var a2 = new[] { 1, 2, 3, 4, 5 };

        var code1 = GetArrayFingerprint(a1);
        var code2 = GetArrayFingerprint(a2);

        Assert.Equal(code1, code2);
    }

    [Fact]
    public void GetDictionaryFingerprint_WhenDictionaryContentIsSame_ShouldReturnStableCode()
    {
        var d1 = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 }
        };
        var d2 = new Dictionary<string, int>
        {
            { "three", 3 },
            { "two", 2 },
            { "one", 1 }
        };
        var result1 = GetDictionaryFingerprint(d1);
        var result2 = GetDictionaryFingerprint(d2);

        Assert.Equal(result1, result2);
    }


    [Fact]
    public void GetObjectFingerprint_ForKnownObjects_ShouldMatchExpectedValues()
    {
        var result1 = GetFingerprint("test");
        var result2 = GetFingerprint(12345);
        var result3 = GetFingerprint<object?>(null);

        Assert.Equal(new(7250582), result1);
        Assert.Equal(new(12345), result2);
        Assert.Equal(Fingerprint.Null, result3);
    }


}
