using System;
using YuukaFlow.Core.Extensions;

namespace YuukaFlow.Core
{
    public readonly struct Fingerprint
    {
        public readonly int Code;

        public Fingerprint(int code)
        {
            Code = code;
        }

        public override readonly string ToString()
        {
            return $"Fingerprint( #{Code:x8} )";
        }

        public static Fingerprint None => new(0);


        public static Fingerprint Combine(Fingerprint fingerprint1, Fingerprint fingerprint2)
        {
            return new Fingerprint(fingerprint1.Code * 31 + fingerprint2.Code);
        }

        public static Fingerprint FromSpan<T>(ReadOnlySpan<T> span)
        {
            if (span == null || span.Length == 0)
                return None;

            var fingerprint = new Fingerprint(span.Length);
            foreach (var b in span)
            {
                fingerprint = fingerprint.Combine(From(b));
            }
            return fingerprint;
        }


        public static Fingerprint From<T>(T obj)
        {
            return obj switch
            {
                null => None,
                IFingerprintProvider persistent => persistent.GetFingerprint(),
                Fingerprint fp => fp,
                string str => FromSpan<char>(str),
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
                Enum e => new Fingerprint(Convert.ToInt32(e)),

                decimal => throw new NotSupportedException($"Type {typeof(T)} is not supported for fingerprinting"),
                float => throw new NotSupportedException($"Type {typeof(T)} is not supported for fingerprinting"),
                double => throw new NotSupportedException($"Type {typeof(T)} is not supported for fingerprinting"),
                _ => throw new NotSupportedException($"Type {typeof(T)} is not supported for fingerprinting"),
            };
        }

        public static Fingerprint FromFingerprintProvider(IFingerprintProvider fingerprintProvider)
        {
            return fingerprintProvider?.GetFingerprint() ?? None;
        }
    }
}