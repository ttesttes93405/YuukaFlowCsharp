using static YuukaFlow.Core.Extensions.FingerprintExtensions;
using static YuukaFlow.Core.Extensions.StringExtensions;

namespace YuukaFlow.Core
{

    public record Flowchart<TName, TPortId> : IFingerprintProvider
    {
        public string Name { get; init; }
        public TName EntryNodeName { get; init; }

        public FlowNode<TName, TPortId>[] FlowNodes { get; init; }

        private Flowchart() => throw new System.Exception("No parameter Flowchart constructor is not allowed");

        public Flowchart(TName entryNodeName, FlowNode<TName, TPortId>[] flowNodes)
        {
            Name = "";
            EntryNodeName = entryNodeName;
            FlowNodes = flowNodes;
        }

        public override string ToString()
        {
            return this.BuildString(new System.Text.StringBuilder()).ToString();
        }

        public Fingerprint GetFingerprint()
        {
            return CombineFingerprint(
                Extensions.FingerprintExtensions.GetFingerprint(Name),
                Extensions.FingerprintExtensions.GetFingerprint(EntryNodeName),
                GetArrayFingerprint(FlowNodes)
            );
        }

        public override int GetHashCode() => GetFingerprint().Code;
    }

}