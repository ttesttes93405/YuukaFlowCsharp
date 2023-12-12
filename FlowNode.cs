

using System;
using System.Collections.Generic;
using System.Text;

namespace YuukaFlow
{
    public record FlowNode : FlowNode<string, string>
    {
        public FlowNode(string name, Dictionary<string, string> outputPorts = null) : base(name, outputPorts) { }
    }

    public record FlowNode<TName, TPortId>
    {
        public TName Name { get; init; }
        public Dictionary<TPortId, TName> OutputPorts { get; init; }

        public FlowNode() { }
        public FlowNode(TName name, Dictionary<TPortId, TName> outputPorts = null)
        {
            Name = name;
            OutputPorts = outputPorts;
        }
    }

}