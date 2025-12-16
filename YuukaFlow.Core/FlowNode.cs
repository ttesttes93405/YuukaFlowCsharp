using System.Collections.Generic;

namespace YuukaFlow.Core
{
    public record FlowNode : FlowNode<string, string>
    {
        public FlowNode(string name, Dictionary<string, string>? outputPorts = null) : base(name, outputPorts) { }
    }

    public record FlowNode<TName, TPortId>
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
    }

}