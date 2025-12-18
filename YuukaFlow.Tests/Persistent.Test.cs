namespace YuukaFlow.Tests;

using static YuukaFlow.Core.Extensions.PersistentExtensions;

public class PersistentTest
{
    [Fact]
    public void TestGetCollectionPersistentCode()
    {
        var c = new int[] { 1, 2, 3, 4, 5 };
        int result = GetArrayPersistentCode(c);

        Assert.Equal(144131870, result);
    }

    [Fact]
    public void TestGetDictionaryPersistentCode()
    {
        var d = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 }
        };
        int result = GetDictionaryPersistentCode(d);

        Assert.Equal(-275003694, result);
    }

    [Fact]
    public void TestGetStringPersistentCode()
    {
        int result = GetStringPersistentCode("hello".AsSpan());

        Assert.Equal(242308077, result);
    }

    [Fact]
    public void TestGetObjectPersistentCode()
    {
        int result1 = GetObjectPersistentCode("test");
        int result2 = GetObjectPersistentCode(12345);
        int result3 = GetObjectPersistentCode<object?>(null);

        Assert.Equal(7250582, result1);
        Assert.Equal(12345, result2);
        Assert.Equal(0, result3);
    }


    [Fact]
    public void TestCombinePersistentCode()
    {
        int result = CombinePersistentCode(123, 456, 789);

        Assert.Equal(222501, result);
    }


    [Fact]
    public void TestCombinePersistentCode_Empty()
    {
        int result = CombinePersistentCode();

        Assert.Equal(0, result);
    }


}