using System;
using System.Linq;
using static YuukaFlow.Core.Extensions.FingerprintExtensions;
using static YuukaFlow.Core.Extensions.StringExtensions;

namespace YuukaFlow.Core
{

    public class Flowchart<TName, TPortId> : IFingerprintProvider
    {
        public string Name { get; init; }
        public TName EntryNodeName { get; init; }

        public FlowNode<TName, TPortId>[] FlowNodes { get; init; }

        private Flowchart() => throw new Exception("No parameter Flowchart constructor is not allowed");

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
            return CombineFingerprints(
                Fingerprint.From(Name),
                Fingerprint.From(EntryNodeName),
                Fingerprint.FromSpan<FlowNode<TName, TPortId>>(FlowNodes)
            );
        }

        public override int GetHashCode() => GetFingerprint().Code;

        public override bool Equals(object obj)
        {
            if (obj is Flowchart<TName, TPortId> other)
            {
                return this.GetFingerprint().Equals(other.GetFingerprint());
            }
            return false;
        }

        public Flowchart<TOtherName, TOtherPortId> ConvertTypes<TOtherName, TOtherPortId>(
            Func<TName, TOtherName> nameConverter,
            Func<TPortId, TOtherPortId> portIdConverter)
        {
            if (nameConverter == null)
                throw new ArgumentNullException(nameof(nameConverter));
            if (portIdConverter == null)
                throw new ArgumentNullException(nameof(portIdConverter));

            return new Flowchart<TOtherName, TOtherPortId>(
                nameConverter(EntryNodeName),
                FlowNodes.Select(n => n.ConvertTypes(nameConverter, portIdConverter)).ToArray()
            )
            {
                Name = this.Name
            };
        }

    }

}