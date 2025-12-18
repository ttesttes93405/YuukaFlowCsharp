using System.Collections.ObjectModel;
using static YuukaFlow.Core.Extensions;

namespace YuukaFlow.Core
{

    public record Flowchart<TName, TPortId> : IPersistent
    {
        public string Name { get; init; }
        public TName EntryNodeName { get; init; }

        public Collection<FlowNode<TName, TPortId>> FlowNodes { get; init; }

        private Flowchart() => throw new System.Exception("No parameter Flowchart constructor is not allowed");

        public Flowchart(TName entryNodeName, Collection<FlowNode<TName, TPortId>> flowNodes)
        {
            Name = "";
            EntryNodeName = entryNodeName;
            FlowNodes = flowNodes;
        }

        public override string ToString()
        {
            return this.BuildString(new System.Text.StringBuilder()).ToString();
        }

        public int GetPersistentCode()
        {
            return CombinePersistentCode(
                GetStringPersistentCode(Name),
                GetObjectPersistentCode(EntryNodeName),
                GetCollectionPersistentCode(FlowNodes)
            );
        }

        public override int GetHashCode() => GetPersistentCode();
    }

}