
using System;
using System.Linq;
using System.Text;

namespace YuukaFlow.Core.Extensions
{
    public static class StringExtensions
    {

        public static StringBuilder BuildString<TName, TPortId>(this Flowchart<TName, TPortId> flowchart, StringBuilder sb, int indent = 0)
        {
            if (flowchart == null)
                throw new ArgumentNullException(nameof(flowchart));

            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            int flowchartHash = flowchart.GetHashCode();

            sb
                .Append(' ', indent).AppendLine($"Flowchart( {flowchart.Name} #{flowchartHash:x8} )")
                .Append(' ', indent).AppendLine("{")
                .Append(' ', indent).AppendLine($"  EntryNodeName = {flowchart.EntryNodeName},")
                .Append(' ', indent).AppendLine("  FlowNodes =")
                .Append(' ', indent).AppendLine("  [");

            foreach (var node in flowchart.FlowNodes)
            {
                node.BuildString(sb, indent + 4);
            }

            sb
                .AppendLine()
                .Append(' ', indent).AppendLine("  ]")
                .Append(' ', indent).AppendLine("}");

            return sb;
        }

        public static string ToString<TName, TPortId>(this Flowchart<TName, TPortId> flowchart, StringBuilder sb, int indent = 0)
        {
            return flowchart
                .BuildString(sb, indent)
                .ToString();
        }

        public static StringBuilder BuildString<TName, TPortId>(this FlowNode<TName, TPortId> flowNode, StringBuilder sb, int indent = 0)
        {
            sb
                .Append(' ', indent).Append($"FlowNode({flowNode.Name})");

            if (flowNode.OutputPorts == null || flowNode.OutputPorts.Count == 0)
                return sb;

            if (flowNode.OutputPorts.Count == 1)
            {
                var kv = flowNode.OutputPorts.First();
                sb
                    .AppendLine($" -|{kv.Key}|-> {kv.Value}");
                return sb;
            }

            sb
                .AppendLine()
                .Append(' ', indent).AppendLine("{");

            foreach (var pair in flowNode.OutputPorts)
            {
                var portId = pair.Key;
                var toName = pair.Value;
                sb
                    .Append(' ', indent + 2).AppendLine($"-|{portId}|-> {toName}");
            }

            sb
                .Append(' ', indent).AppendLine("}");

            return sb;
        }

        public static string ToString<TName, TPortId>(this FlowNode<TName, TPortId> flowNode, StringBuilder sb, int indent = 0)
        {
            return flowNode
                .BuildString(sb, indent)
                .ToString();
        }


    }
}