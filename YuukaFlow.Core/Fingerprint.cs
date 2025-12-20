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

        public static Fingerprint Null => new(0);
    }
}