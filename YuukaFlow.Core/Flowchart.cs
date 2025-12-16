using System.Collections.ObjectModel;

namespace YuukaFlow.Core
{

    public record Flowchart<TName, TPortId>
    {
        public TName EntryNodeName { get; init; }

        public Collection<FlowNode<TName, TPortId>> FlowNodes { get; init; }

        private Flowchart() => throw new System.Exception("Flowchart must have an entry node and flow nodes");
        public Flowchart(TName entryNodeName, Collection<FlowNode<TName, TPortId>> flowNodes)
        {
            EntryNodeName = entryNodeName;
            FlowNodes = flowNodes;
        }
    }

}