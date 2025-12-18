using System;
using System.Collections.Generic;
using static YuukaFlow.Core.Extensions.Persistent;
using static YuukaFlow.Core.Extensions.Text;

namespace YuukaFlow.Core
{
    public record FlowNode : FlowNode<string, string>
    {
        public FlowNode(string name) : base(name) { }
        public FlowNode(string name, params (string portId, string targetNode)[] outputPorts) : base(name, outputPorts) { }
    }

    public record FlowNode<TName, TPortId> : IPersistent
    {
        public TName Name { get; init; }
        internal Dictionary<TPortId, TName>? OutputPorts { get; private set; }

        public bool HasOutputPorts => OutputPorts != null && OutputPorts.Count > 0;

        public bool TryGetNextNodeName(TPortId portId, out TName nextNode)
        {
            if (OutputPorts != null && OutputPorts.TryGetValue(portId, out nextNode))
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
            OutputPorts = null;
        }

        public FlowNode(TName name, params (TPortId portId, TName targetNode)[] outputPorts)
        {
            Name = name;
            if (outputPorts.Length == 0)
            {
                OutputPorts = null;
            }
            else
            {
                OutputPorts = new Dictionary<TPortId, TName>();
                foreach (var (portId, targetNode) in outputPorts)
                {
                    OutputPorts[portId] = targetNode;
                }
            }
        }


        public override string ToString()
        {
            return this.BuildString(new System.Text.StringBuilder()).ToString();
        }

        public int GetPersistentCode()
        {
            return CombinePersistentCode(
                Extensions.Persistent.GetPersistentCode(Name),
                GetDictionaryPersistentCode(OutputPorts)
            );
        }

        public override int GetHashCode() => GetPersistentCode();
    }

}