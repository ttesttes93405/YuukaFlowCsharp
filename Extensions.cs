using System;
using System.Text;
using System.Collections.Generic;

namespace YuukaFlow
{
    public static class Extensions
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



        public static string ConvertToCode<TName, TPortId>(
                this Flowchart<TName, TPortId> flowchart,
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
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"{nameof(flowchart.EntryNodeName)} = {nameSerializer(flowchart.EntryNodeName)},")
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"{nameof(flowchart.FlowNodes)} = new()")
                .Append(indentString).Append(INDENT_UNIT).AppendLine("{");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"{node.ConvertToCode(nameSerializer, portIdSerializer, indent + 1)},");
            }

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine("}")
                .Append(indentString).Append("};");

            return stringBuilder.ToString();
        }



        public static string GetImplementationCodeTemplate<TName, TPortId>(
            this Flowchart<TName, TPortId> flowchart,
            Func<TName, string> nameSerializer,
            Func<TPortId, string> portIdSerializer,
            Func<TName, string> implementationNameSerializer,
            string nameTypeName,
            string portIdTypeName,
            string contextTypeName,
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
                .Append(indentString).AppendLine($"Dictionary<{nameTypeName}, Func<{contextTypeName}, Task<{portIdTypeName}>>> GetImplementations()")
                .Append(indentString).AppendLine("{");

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine($"return new()")
                .Append(indentString).Append(INDENT_UNIT).AppendLine("{");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .Append(indentString).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"[{nameSerializer(nodeName)}] = {implementationNameSerializer(nodeName)},");
            }

            stringBuilder
                .Append(indentString).Append(INDENT_UNIT).AppendLine("};");



            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .AppendLine()
                    .Append(indentString).Append(INDENT_UNIT).AppendLine($"async Task<{portIdTypeName}> {implementationNameSerializer(nodeName)}({contextTypeName} context)")
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