
using System;
using System.Collections.Generic;
using System.Linq;

namespace YuukaFlow.Core.Extensions
{
    public static class PersistentExtensions
    {

        ref struct HashJumper
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

        }

        public static int GetArrayPersistentCode<T>(T[] array)
        {
            if (array == null)
                return 0;

            var hashJumper = new HashJumper(array.Length, seed: 31);
            foreach (var item in array)
            {
                hashJumper.Jump(GetObjectPersistentCode(item));
            }
            return hashJumper.Value;
        }

        public static int GetDictionaryPersistentCode<TKey, TValue>(Dictionary<TKey, TValue>? dict)
        {
            if (dict == null)
                return 0;

            var hashJumper = new HashJumper(17, seed: 31);

            foreach (var pair in dict.OrderBy(kv => kv.Key))
            {
                hashJumper.Jump(GetObjectPersistentCode(pair.Key));
                hashJumper.Jump(GetObjectPersistentCode(pair.Value));
            }
            return hashJumper.Value;
        }

        public static int GetObjectPersistentCode<T>(T obj)
        {
            return obj switch
            {
                null => 0,
                IPersistent persistent => persistent.GetPersistentCode(),
                string str => GetStringPersistentCode(str),
                _ => obj.GetHashCode(), // not persistent but best effort
            };
        }

        public static int GetStringPersistentCode(ReadOnlySpan<char> str)
        {
            if (str.Length == 0)
                return 0;

            var hashJumper = new HashJumper(str.Length, seed: 31);
            foreach (var ch in str)
            {
                hashJumper.Jump(ch);
            }
            return hashJumper.Value;
        }

        public static int CombinePersistentCode(params int[] codes)
        {
            return GetArrayPersistentCode(codes);
        }




    }
}