using System.Collections.ObjectModel;
using System.Linq;

namespace YuukaFlow.Core
{

    public record Flowchart<TName, TPortId>
    {
        public TName EntryNodeName { get; init; }

        public Collection<FlowNode<TName, TPortId>> FlowNodes { get; init; }

        private Flowchart() => throw new System.Exception("No parameter Flowchart constructor is not allowed");

        public Flowchart(TName entryNodeName, Collection<FlowNode<TName, TPortId>> flowNodes)
        {
            EntryNodeName = entryNodeName;
            FlowNodes = flowNodes;
        }

        public override string ToString()
        {
            return
                $"Flowchart\n" +
                $"{{\n" +
                $"  EntryNodeName={EntryNodeName},\n" +
                $"  FlowNodes=[\n" +
                $"{string.Join(",\n", FlowNodes.Select(f => $"    {f}"))}\n" +
                $"  ]\n" +
                $"}}"
                ;
        }
    }

}