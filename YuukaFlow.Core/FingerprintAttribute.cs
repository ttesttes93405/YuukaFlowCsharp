using System;

namespace YuukaFlow.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class FingerprintAttribute : Attribute
    {
        public Fingerprint Fingerprint { get; }

        public FingerprintAttribute(string hex)
        {
            Fingerprint = new Fingerprint(unchecked((int)Convert.ToUInt32(hex, 16)));
        }
    }
}
