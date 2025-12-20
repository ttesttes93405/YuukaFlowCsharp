
using System;

namespace YuukaFlow.Core.Extensions
{
    public static class FingerprintExtensions
    {
        public static Fingerprint GetArrayFingerprint<T>(ReadOnlySpan<T> span)
        {
            return Fingerprint.FromSpan(span);
        }

        public static Fingerprint CombineFingerprints(params Fingerprint[] fingerprints)
        {
            return GetArrayFingerprint<Fingerprint>(fingerprints);
        }

        public static Fingerprint Combine(this Fingerprint fingerprint1, Fingerprint fingerprint2)
        {
            return Fingerprint.Combine(fingerprint1, fingerprint2);
        }
    }
}