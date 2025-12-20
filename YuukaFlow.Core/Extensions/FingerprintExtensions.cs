
using System;
using System.Collections.Generic;
using System.Linq;

namespace YuukaFlow.Core.Extensions
{
    public static class FingerprintExtensions
    {

        public ref struct HashJumper
        {
            readonly private int _seed;
            private int _hash;
            public readonly int Value => _hash;

            public HashJumper(int initialHash, int seed)
            {
                _hash = initialHash;
                _seed = seed;
            }

            public int Jump(int jump)
            {
                _hash = _hash * _seed + jump;
                return _hash;
            }

            public int Jump(Fingerprint fingerprint)
            {
                return Jump(fingerprint.Code);
            }


        }

        public static Fingerprint GetArrayFingerprint<T>(T[] array)
        {
            if (array == null)
                return Fingerprint.Null;

            var hashJumper = new HashJumper(array.Length, seed: 31);
            foreach (var item in array)
            {
                hashJumper.Jump(GetFingerprint(item));
            }
            return new(hashJumper.Value);
        }

        public static Fingerprint GetDictionaryFingerprint<TKey, TValue>(Dictionary<TKey, TValue>? dict)
        {
            if (dict == null)
                return Fingerprint.Null;

            var hashJumper = new HashJumper(17, seed: 31);

            foreach (var pair in dict.OrderBy(kv => kv.Key))
            {
                hashJumper.Jump(GetFingerprint(pair.Key));
                hashJumper.Jump(GetFingerprint(pair.Value));
            }
            return new(hashJumper.Value);
        }

        public static Fingerprint GetFingerprint<T>(T obj)
        {
            return obj switch
            {
                null => Fingerprint.Null,
                IFingerprintProvider persistent => persistent.GetFingerprint(),
                Fingerprint fp => fp,
                string str => GetStringFingerprint(str),
                char ch => new Fingerprint(ch),
                bool b => new Fingerprint(b ? 1231 : 1237),
                byte b => new Fingerprint(b),
                sbyte sb => new Fingerprint(sb),
                short s => new Fingerprint(s),
                ushort us => new Fingerprint(us),
                int i => new Fingerprint(i),
                uint ui => new Fingerprint((int)ui),
                long l => new Fingerprint((int)(l ^ (l >> 32))),
                ulong ul => new Fingerprint((int)(ul ^ (ul >> 32))),
                float f => new Fingerprint(BitConverter.ToInt32(BitConverter.GetBytes(f), 0)),
                double d => new Fingerprint((int)(BitConverter.DoubleToInt64Bits(d) ^ (BitConverter.DoubleToInt64Bits(d) >> 32))),
                Enum e => new Fingerprint(Convert.ToInt32(e)),
                _ => throw new NotSupportedException($"Type {typeof(T)} is not supported for fingerprinting"),
            };

            static Fingerprint GetStringFingerprint(ReadOnlySpan<char> str)
            {
                if (str.Length == 0)
                    return Fingerprint.Null;

                var hashJumper = new HashJumper(str.Length, seed: 31);
                foreach (var ch in str)
                {
                    hashJumper.Jump(ch);
                }
                return new Fingerprint(hashJumper.Value);
            }
        }


        public static Fingerprint CombineFingerprint(params Fingerprint[] fintgerprints)
        {
            return GetArrayFingerprint(fintgerprints);
        }




    }
}