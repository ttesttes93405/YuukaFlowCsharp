using System;
using System.Collections.Generic;
using static YuukaFlow.Core.Extensions.FingerprintExtensions;
using static YuukaFlow.Core.Extensions.StringExtensions;

namespace YuukaFlow.Core
{
    public record FlowNode : FlowNode<string, string>
    {
        public FlowNode(string name) : base(name) { }
        public FlowNode(string name, params (string portId, string targetNode)[] outputPorts) : base(name, outputPorts) { }
    }

    public record FlowNode<TName, TPortId> : IFingerprintProvider
    {
        public TName Name { get; init; }
        internal TinyDictionary<TPortId, TName> OutputPorts { get; private set; }

        public bool HasOutputPorts => OutputPorts.Count > 0;

        public bool TryGetNextNodeName(TPortId portId, out TName nextNode)
        {
            if (OutputPorts.TryGetValue(portId, out nextNode))
            {
                return true;
            }
            nextNode = default!;
            return false;
        }

        private FlowNode() => throw new Exception("FlowNode must have a name");

        public FlowNode(TName name)
        {
            Name = name;
            OutputPorts = new();
        }

        public FlowNode(TName name, params (TPortId portId, TName targetNode)[] outputPorts)
        {
            Name = name;
            if (outputPorts.Length == 0)
            {
                OutputPorts = new();
            }
            else
            {
                OutputPorts = new(outputPorts);
            }
        }


        public override string ToString()
        {
            return this.BuildString(new System.Text.StringBuilder()).ToString();
        }

        public Fingerprint GetFingerprint()
        {
            return CombineFingerprints(
                Fingerprint.From(Name),
                Fingerprint.FromFingerprintProvider(OutputPorts)
            );
        }

        public override int GetHashCode() => GetFingerprint().Code;
    }

}