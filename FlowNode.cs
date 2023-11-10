

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


    public static class FlowNodeExtensions
    {

        public static string ConvertToCode<TName, TPortId>(
                this FlowNode<TName, TPortId> flowNode,
                Func<TName, string> nameSerializer,
                Func<TPortId, string> portIdSerializer,
                int indent = 0)
        {
            const string INDENT_UNIT = "    ";
            var indentStringBuilder = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                indentStringBuilder.Append(INDENT_UNIT);
            }
            string indentString = indentStringBuilder.ToString();

            bool isOutputPortEmpty = flowNode.OutputPorts == null || flowNode.OutputPorts.Count == 0;

            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append($"new({nameSerializer(flowNode.Name)}");

            if (isOutputPortEmpty == false)
            {
                stringBuilder
                    .AppendLine(", new()")
                    .Append(indentString).Append(INDENT_UNIT).AppendLine("{");


                foreach (var pair in flowNode.OutputPorts)
                {
                    var portId = pair.Key;
                    var toName = pair.Value;
                    stringBuilder
                        .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"[{portIdSerializer(portId)}] = {nameSerializer(toName)},");
                }
            }


            if (isOutputPortEmpty == false)
            {
                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).Append("}");
            }

            stringBuilder
                .Append(")");


            return stringBuilder.ToString();
        }
    }

}