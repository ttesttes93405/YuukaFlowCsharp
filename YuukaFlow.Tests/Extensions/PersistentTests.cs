namespace YuukaFlow.Tests.Extensions;

using static YuukaFlow.Core.Extensions.Persistent;

public class PersistentTests
{

    [Theory]
    [InlineData(null, 0)]
    [InlineData(6, 6)]
    [InlineData("hello", 242308077)]
    public void GetObjectPersistentCode_ShouldMatchExpectedValue(object? obj, int expected)
    {
        int result = GetPersistentCode(obj);
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(new[] { 123, 456, 789 }, 222501)]
    [InlineData(new int[0], 0)]
    public void CombinePersistentCode_ShouldMatchExpectedValue(int[] value, int expected)
    {
        int result = CombinePersistentCode(value);
        Assert.Equal(expected, result);
    }


    [Fact]
    public void GetArrayPersistentCode_WhenArrayContentIsSame_ShouldReturnStableCode()
    {
        var a1 = new[] { 1, 2, 3, 4, 5 };
        var a2 = new[] { 1, 2, 3, 4, 5 };

        int code1 = GetArrayPersistentCode(a1);
        int code2 = GetArrayPersistentCode(a2);

        Assert.Equal(code1, code2);
    }

    [Fact]
    public void GetDictionaryPersistentCode_WhenDictionaryContentIsSame_ShouldReturnStableCode()
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
        int result1 = GetDictionaryPersistentCode(d1);
        int result2 = GetDictionaryPersistentCode(d2);

        Assert.Equal(result1, result2);
    }


    [Fact]
    public void GetObjectPersistentCode_ForKnownObjects_ShouldMatchExpectedValues()
    {
        int result1 = GetPersistentCode("test");
        int result2 = GetPersistentCode(12345);
        int result3 = GetPersistentCode<object?>(null);

        Assert.Equal(7250582, result1);
        Assert.Equal(12345, result2);
        Assert.Equal(0, result3);
    }


}
