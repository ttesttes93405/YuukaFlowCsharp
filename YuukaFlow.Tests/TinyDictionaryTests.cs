namespace YuukaFlow.Tests;

using System.Globalization;
using YuukaFlow.Core;

public class TinyDictionaryTests
{

    [Fact]
    public void GetFingerprint_IsStableAcrossCultures()
    {
        var items = new (string key, string value)[]
        {
            ("Apple", "1"),
            ("apple", "2"),
            ("coop", "3"),
            ("co-op", "4"),
            ("café", "5"),
            ("cafe", "6"),
        };

        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var invariantFingerprint = new TinyDictionary<string, string>(items).GetFingerprint();

            // In globalization-invariant deployments (e.g. DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1,
            // common in trimmed containers), only the invariant culture exists, so
            // there is nothing further to compare against.
            CultureInfo deDe, zhTw;
            try
            {
                deDe = new CultureInfo("de-DE");
                zhTw = new CultureInfo("zh-TW");
            }
            catch (CultureNotFoundException)
            {
                return;
            }

            CultureInfo.CurrentCulture = deDe;
            var deDeFingerprint = new TinyDictionary<string, string>(items).GetFingerprint();

            CultureInfo.CurrentCulture = zhTw;
            var zhTwFingerprint = new TinyDictionary<string, string>(items).GetFingerprint();

            Assert.Equal(invariantFingerprint, deDeFingerprint);
            Assert.Equal(invariantFingerprint, zhTwFingerprint);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }


    [Fact]
    public void GetFingerprint_IsIndependentOfInsertionOrder()
    {
        var itemsA = new (string key, string value)[]
        {
            ("ok", "NodeB"),
            ("co-op", "NodeC"),
            ("coop", "NodeD"),
        };
        var itemsB = new (string key, string value)[]
        {
            ("coop", "NodeD"),
            ("ok", "NodeB"),
            ("co-op", "NodeC"),
        };

        var fingerprintA = new TinyDictionary<string, string>(itemsA).GetFingerprint();
        var fingerprintB = new TinyDictionary<string, string>(itemsB).GetFingerprint();

        Assert.Equal(fingerprintA, fingerprintB);
    }


    [Fact]
    public void GetFingerprint_DoesNotRequireKeyToImplementIComparable()
    {
        // NonComparableKey deliberately implements IFingerprintProvider (which
        // GetFingerprint() always needed to combine key/value pairs) but not
        // IComparable, to prove sorting no longer depends on IComparable.
        var items = new (NonComparableKey key, string value)[]
        {
            (new NonComparableKey("a"), "X"),
            (new NonComparableKey("b"), "Y"),
        };

        var exception = Record.Exception(() => new TinyDictionary<NonComparableKey, string>(items).GetFingerprint());

        Assert.Null(exception);
    }


    private sealed class NonComparableKey : IFingerprintProvider
    {
        public string Value { get; }
        public NonComparableKey(string value) => Value = value;
        public override bool Equals(object? obj) => obj is NonComparableKey other && other.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
        public Fingerprint GetFingerprint() => Fingerprint.From(Value);
    }

}
