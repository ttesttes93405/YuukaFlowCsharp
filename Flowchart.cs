using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YuukaFlow
{

    public record Flowchart<TName, TPortId>
    {
        public TName EntryNodeName { get; init; }

        public Collection<FlowNode<TName, TPortId>> FlowNodes { get; init; }

    }

}