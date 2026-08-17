
using System;

namespace YuukaFlow.Core.Extensions
{
    public static class CodeTemplateExtensions
    {


        public static string ConvertToCode<TName, TPortId>(
                this FlowNode<TName, TPortId> flowNode,
                Func<TName, string> nameSerializer,
                Func<TPortId, string> portIdSerializer,
                int indent = 0)
        {
            var builder = new IndentBuilder(indent);

            builder.Append($"new({nameSerializer(flowNode.Name)}");

            if (flowNode.OutputPorts.Count > 0)
            {
                foreach (var (portId, toName) in flowNode.OutputPorts.Pairs)
                {
                    builder
                        .AppendLine(",")
                        .Append(2, $"({portIdSerializer(portId)}, {nameSerializer(toName)})");
                }

                builder
                    .AppendLine()
                    .Append(1, "");
            }

            builder.Append(")");

            return builder.ToString();
        }



        public static string ConvertToCode<TName, TPortId>(
                this Flowchart<TName, TPortId> flowchart,
                Func<TName, string> nameSerializer,
                Func<TPortId, string> portIdSerializer,
                string nameTypeName,
                string portIdTypeName,
                int indent = 0)
        {
            var builder = new IndentBuilder(indent);

            builder
                .AppendLine(0, $"static Flowchart<{nameTypeName}, {portIdTypeName}> GetFlowchart()")
                .AppendLine(0, "{");

            builder
                .AppendLine(1, $"return new(")
                .AppendLine(2, $"entryNodeName: {nameSerializer(flowchart.EntryNodeName)},")
                .AppendLine(2, $"flowNodes: new FlowNode<{nameTypeName}, {portIdTypeName}>[]")
                .AppendLine(2, "{");

            foreach (var node in flowchart.FlowNodes)
            {
                builder.AppendLine(3, $"{node.ConvertToCode(nameSerializer, portIdSerializer, indent + 2)},");
            }

            builder
                .AppendLine(2, "}")
                .Append(1, ")");

            if (string.IsNullOrEmpty(flowchart.Name) == false)
            {
                builder
                    .AppendLine()
                    .Append(1, "{")
                    .AppendLine()
                    .Append(2, $"Name = \"{flowchart.Name}\",")
                    .AppendLine()
                    .Append(1, "}");
            }

            builder
                .AppendLine(";")
                .Append(0, "}");

            return builder.ToString();
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
            var builder = new IndentBuilder(indent);

            builder
                .AppendLine(0, $"[Fingerprint(\"{flowchart.GetFingerprint().Code:x8}\")]")
                .AppendLine(0, $"static Dictionary<{nameTypeName}, FlowNodeImplementation<{contextTypeName}, {portIdTypeName}>> GetImplementations()")
                .AppendLine(0, "{")
                .AppendLine(1, $"return new()")
                .AppendLine(1, "{");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;
                builder.AppendLine(2, $"[{nameSerializer(nodeName)}] = {implementationNameSerializer(nodeName)},");
            }

            builder.AppendLine(1, "};");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                builder
                    .AppendLine()
                    .AppendLine(1, $"static async ValueTask<{portIdTypeName}> {implementationNameSerializer(nodeName)}({contextTypeName} context)")
                    .AppendLine(1, "{");

                if (node.OutputPorts.Count == 0)
                {
                    builder.AppendLine(2, $"return default;");
                }
                else
                {
                    foreach (var (portId, _) in node.OutputPorts.Pairs)
                    {
                        builder.AppendLine(2, $"return {portIdSerializer(portId)};");
                    }
                }

                builder.AppendLine(1, "}");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }


    }
}