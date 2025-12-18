using static YuukaFlow.Core.Extensions.Persistent;
using static YuukaFlow.Core.Extensions.Text;

namespace YuukaFlow.Core
{

    public record Flowchart<TName, TPortId> : IPersistent
    {
        public string Name { get; init; }
        public TName EntryNodeName { get; init; }

        public FlowNode<TName, TPortId>[] FlowNodes { get; init; }

        private Flowchart() => throw new System.Exception("No parameter Flowchart constructor is not allowed");

        public Flowchart(TName entryNodeName, FlowNode<TName, TPortId>[] flowNodes)
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
                Extensions.Persistent.GetPersistentCode(Name),
                Extensions.Persistent.GetPersistentCode(EntryNodeName),
                GetArrayPersistentCode(FlowNodes)
            );
        }

        public override int GetHashCode() => GetPersistentCode();
    }

}