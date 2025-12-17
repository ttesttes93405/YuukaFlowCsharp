using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
        {
            if (OutputPorts == null || OutputPorts.Count == 0)
                return $"FlowNode({Name})";

            if (OutputPorts.Count == 1)
            {
                var kv = OutputPorts.First();
                return
                    $"FlowNode({Name}) -|{kv.Key}|-> {kv.Value}";
            }

            return
                $"FlowNode({Name})\n" +
                $"{{\n" +
                $"{string.Join(", ", OutputPorts.Select(kv => $"  -|{kv.Key}|-> {kv.Value}"))}\n" +
                $"}}"
                ;
        }
    }

}