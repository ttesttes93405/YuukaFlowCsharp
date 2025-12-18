using System.Collections.Generic;
using static YuukaFlow.Core.Extensions;

namespace YuukaFlow.Core
{
    public record FlowNode : FlowNode<string, string>
    {
        public FlowNode(string name, Dictionary<string, string>? outputPorts = null) : base(name, outputPorts) { }
    }

    public record FlowNode<TName, TPortId> : IPersistent
    {
        public TName Name { get; init; }
        public Dictionary<TPortId, TName>? OutputPorts { get; init; }

        private FlowNode() => throw new System.Exception("FlowNode must have a name");

        public FlowNode(TName name)
        {
            Name = name;
            OutputPorts = null;
        }
        public FlowNode(TName name, Dictionary<TPortId, TName>? outputPorts = null)
        {
            Name = name;
            OutputPorts = outputPorts;
        }

        public override string ToString()
        {
            return this.BuildString(new System.Text.StringBuilder()).ToString();
        }
        public int GetPersistentCode()
        {
            return CombinePersistentCode(
                GetObjectPersistentCode(Name),
                GetDictionaryPersistentCode(OutputPorts)
            );
        }

        public override int GetHashCode() => GetPersistentCode();
    }

}