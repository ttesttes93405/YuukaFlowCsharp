
using System;
using System.Collections.Generic;
using System.Text;

namespace YuukaFlow
{

    public record FlowDocument<TName, TPortId>
    {
        public TName EntryNodeName { get; init; }
        public Dictionary<TName, FlowNode<TName, TPortId>> FlowNodes { get; init; }

    }

    public static class FlowDocumentExtensions
    {

        public static string ConvertToCode<TName, TPortId>(
                this FlowDocument<TName, TPortId> flowDoc,
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


            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"new()")
                .AppendLine("{")
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"{nameof(flowDoc.EntryNodeName)} = {nameSerializer(flowDoc.EntryNodeName)},")
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"{nameof(flowDoc.FlowNodes)} = new()")
                .Append(indentString).Append(INDENT_UNIT).AppendLine("{");

            foreach (var pair in flowDoc.FlowNodes)
            {
                var nodeName = pair.Key;
                var node = pair.Value;
                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"[{nameSerializer(nodeName)}] = {node.ConvertToCode(nameSerializer, portIdSerializer, indent + 1)},");
            }

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine("}")
                .Append(indentString).Append("};");

            return stringBuilder.ToString();
        }

        public static string GetActivityCodeTemplate<TName, TPortId>(
            this FlowDocument<TName, TPortId> flowDoc,
            Func<TName, string> nameSerializer,
            Func<TPortId, string> portIdSerializer,
            Func<TName, string> activityFuncNameSerializer,
            string nameTypeName,
            string portIdTypeName,
            string stateTypeName,
            int indent = 0
        )
        {
            const string INDENT_UNIT = "    ";
            var indentStringBuilder = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                indentStringBuilder.Append(INDENT_UNIT);
            }
            string indentString = indentStringBuilder.ToString();

            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(indentString).AppendLine($"Dictionary<{nameTypeName}, Func<{stateTypeName}, Task<{portIdTypeName}>>> GetActivities()")
                .Append(indentString).AppendLine("{");

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"return new()")
                .Append(indentString).Append(INDENT_UNIT).AppendLine("{");

            foreach (var nodeName in flowDoc.FlowNodes.Keys)
            {
                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"[{nameSerializer(nodeName)}] = {activityFuncNameSerializer(nodeName)},");
            }

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine("};");



            foreach (var (nodeName, node) in flowDoc.FlowNodes)
            {
                stringBuilder
                    .AppendLine()
                    .Append(indentString).Append(INDENT_UNIT).AppendLine($"async Task<{portIdTypeName}> {activityFuncNameSerializer(nodeName)}({stateTypeName} state)")
                    .Append(indentString).Append(INDENT_UNIT).AppendLine("{");

                if (node.OutputPorts == null || node.OutputPorts.Count == 0)
                {
                    stringBuilder
                        .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"return default;");
                }
                else
                {
                    foreach (var (portId, port) in node.OutputPorts)
                    {
                        stringBuilder
                            .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"return {portIdSerializer(portId)};");
                    }
                }

                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).AppendLine("}");

            }

            stringBuilder
                .AppendLine("}");

            return stringBuilder.ToString();
        }
    }
}