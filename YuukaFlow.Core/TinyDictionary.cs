using System.Collections.Generic;
using System.Linq;
using YuukaFlow.Core.Extensions;

namespace YuukaFlow.Core
{

    public readonly struct TinyDictionary<TKey, TValue> : IFingerprintProvider
    {
        private readonly (TKey key, TValue value)[]? items;

        public TinyDictionary(params (TKey key, TValue value)[] items)
        {
            if (items.Length == 0)
            {
                this.items = null;
                return;
            }

            if (CheckKey(items))
            {
                throw new System.ArgumentException("Duplicate keys are not allowed in TinyDictionary.");
            }

            this.items = items;

            static bool CheckKey((TKey key, TValue value)[] items)
            {
                var seenKeys = new HashSet<TKey>();
                foreach (var (key, _) in items)
                {
                    if (key == null)
                    {
                        throw new System.ArgumentNullException("Key in TinyDictionary cannot be null.");
                    }
                    else if (seenKeys.Contains(key))
                    {
                        return true;
                    }
                    else
                    {
                        seenKeys.Add(key);
                    }
                }
                return false;
            }
        }

        public readonly int Count => items == null ? 0 : items.Length;

        public readonly IEnumerable<TKey> Keys
        {
            get
            {
                if (items == null)
                    yield break;

                foreach (var (key, _) in items)
                {
                    yield return key;
                }
            }
        }

        public readonly IEnumerable<TValue> Values
        {
            get
            {
                if (items == null)
                    yield break;
                foreach (var (_, value) in items)
                {
                    yield return value;
                }
            }
        }

        public readonly IEnumerable<(TKey key, TValue value)> Pairs
        {
            get
            {
                if (items == null)
                    yield break;
                foreach (var (key, value) in items)
                {
                    yield return (key, value);
                }
            }
        }


        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                    return value;

                throw new KeyNotFoundException($"The given key '{key}' was not present in the TinyDictionary.");
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new System.ArgumentNullException("Key in TinyDictionary cannot be null.");
            }
            if (items != null)
            {
                foreach ((TKey k, TValue v) in items)
                {
                    if (k!.GetHashCode() == key.GetHashCode() &&
                        EqualityComparer<TKey>.Default.Equals(k, key))
                    {
                        value = v;
                        return true;
                    }
                }
            }
            value = default!;
            return false;
        }

        public Fingerprint GetFingerprint()
        {
            if (items == null)
                return Fingerprint.None;

            var fingerprint = new Fingerprint(17);

            foreach (var (key, value) in items.OrderBy(kv => kv.key))
            {
                fingerprint = fingerprint
                    .Combine(Fingerprint.From(key))
                    .Combine(Fingerprint.From(value));
            }
            return fingerprint;
        }

        public override int GetHashCode() => GetFingerprint().Code;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();
            if (items != null)
            {
                foreach (var (key, value) in items)
                {
                    dict[key] = value;
                }
            }
            return dict;
        }

    }
}