
using System;
using System.Linq;
using System.Text;

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
            const string INDENT_UNIT = "    ";
            string baseIndent = INDENT_UNIT.Repeat(indent);

            bool isOutputPortEmpty = flowNode.OutputPorts.Count == 0;

            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append($"new({nameSerializer(flowNode.Name)}");

            if (isOutputPortEmpty == false)
            {
                foreach (var (portId, toName) in flowNode.OutputPorts.Pairs)
                {
                    stringBuilder
                        .AppendLine(",")
                        .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT)
                        .Append('(')
                        // .Append("portId: ")
                        .Append(portIdSerializer(portId))
                        .Append(", ")
                        // .Append("targetNode: ")
                        .Append(nameSerializer(toName))
                        .Append(')');
                }

                stringBuilder
                    .AppendLine()
                    .Append(baseIndent).Append(INDENT_UNIT);
            }


            stringBuilder
                .Append(")");


            return stringBuilder.ToString();
        }



        public static string ConvertToCode<TName, TPortId>(
                this Flowchart<TName, TPortId> flowchart,
                Func<TName, string> nameSerializer,
                Func<TPortId, string> portIdSerializer,
                string nameTypeName,
                string portIdTypeName,
                int indent = 0)
        {
            const string INDENT_UNIT = "    ";
            string baseIndent = INDENT_UNIT.Repeat(indent);


            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(baseIndent).AppendLine($"static Flowchart<{nameTypeName}, {portIdTypeName}> GetFlowchart()")
                .Append(baseIndent).AppendLine("{");

            stringBuilder
                .Append(baseIndent).Append(INDENT_UNIT).AppendLine($"return new(")
                .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"entryNodeName: {nameSerializer(flowchart.EntryNodeName)},")
                .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"flowNodes: new FlowNode<{nameTypeName}, {portIdTypeName}>[]")
                .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine("{");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"{node.ConvertToCode(nameSerializer, portIdSerializer, indent + 2)},");
            }

            stringBuilder
                .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine("}")
                .Append(baseIndent).Append(INDENT_UNIT).Append(")");


            if (string.IsNullOrEmpty(flowchart.Name) == false)
            {
                stringBuilder
                    .AppendLine()
                    .Append(baseIndent).Append(INDENT_UNIT).Append('{')
                    .AppendLine()
                    .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).Append($"Name = \"{flowchart.Name}\",")
                    .AppendLine()
                    .Append(baseIndent).Append(INDENT_UNIT).Append('}');
            }

            stringBuilder
                .Append(baseIndent).AppendLine(";");

            stringBuilder
                .Append(baseIndent).Append("}");

            return stringBuilder.ToString();
        }


        public class TemplateFlowNode
        {
            public string Name { get; set; } = "--";
            public TemplatePort[] OutputPorts { get; set; } = Array.Empty<TemplatePort>();

            public class TemplatePort
            {
                public string PortId { get; set; } = "--";
                public string TargetNode { get; set; } = "--";

                public TemplatePort(string portId, string targetNode)
                {
                    PortId = portId;
                    TargetNode = targetNode;
                }
            }

            public override string ToString()
            {
                return $"TemplateFlowNode(Name={Name}, OutputPorts=[{string.Join(", ", OutputPorts.Select(p => $"(PortId={p.PortId}, TargetNode={p.TargetNode})"))}])";
            }
        }

        public static string ConvertToTemplateCode<TName, TPortId>(
            this Flowchart<TName, TPortId> flowchart,
            Func<TName, string> nameSerializer,
            Func<TPortId, string> portIdSerializer,
            string nameTypeName,
            string portIdTypeName,
            int indent = 0
        )
        {
            if (flowchart == null)
                throw new ArgumentNullException(nameof(flowchart));

            var name = "Flowchart";

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"YuukaFlow.Core.Templates.{name}.scriban";

            using var stream = asm.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Template not found: {name}");

            using var reader = new System.IO.StreamReader(stream);
            var template = reader.ReadToEnd();

            var context = new Scriban.Runtime.ScriptObject()
            {
                ["EntryNodeName"] = flowchart.EntryNodeName,
                ["FlowNodes"] = flowchart.FlowNodes.Select(node => new TemplateFlowNode
                {
                    Name = nameSerializer(node.Name),
                    OutputPorts = node.OutputPorts.Pairs
                        .Select(pair => new TemplateFlowNode.TemplatePort(portIdSerializer(pair.key), nameSerializer(pair.value)))
                        .ToArray()
                }).ToArray(),
                ["TName"] = nameTypeName,
                ["TPortId"] = portIdTypeName,
            };
            var templateContext = new Scriban.TemplateContext();
            var parsedTemplate = Scriban.Template.Parse(template);
            templateContext.PushGlobal(context);
            return parsedTemplate.Render(templateContext);
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
            string baseIndent = INDENT_UNIT.Repeat(indent);

            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(baseIndent).AppendLine($"static Dictionary<{nameTypeName}, FlowNodeImplementation<{contextTypeName}, {portIdTypeName}>> GetImplementations()")
                .Append(baseIndent).AppendLine("{");

            stringBuilder
                .Append(baseIndent).Append(INDENT_UNIT).AppendLine($"return new()")
                .Append(baseIndent).Append(INDENT_UNIT).AppendLine("{");

            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"[{nameSerializer(nodeName)}] = {implementationNameSerializer(nodeName)},");
            }

            stringBuilder
                .Append(baseIndent).Append(INDENT_UNIT).AppendLine("};");



            foreach (var node in flowchart.FlowNodes)
            {
                var nodeName = node.Name;

                stringBuilder
                    .AppendLine()
                    .Append(baseIndent).Append(INDENT_UNIT).AppendLine($"static async ValueTask<{portIdTypeName}> {implementationNameSerializer(nodeName)}({contextTypeName} context)")
                    .Append(baseIndent).Append(INDENT_UNIT).AppendLine("{");

                if (node.OutputPorts.Count == 0)
                {
                    stringBuilder
                        .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"return default;");
                }
                else
                {
                    foreach (var (portId, _) in node.OutputPorts.Pairs)
                    {
                        stringBuilder
                            .Append(baseIndent).Append(INDENT_UNIT).Append(INDENT_UNIT).AppendLine($"return {portIdSerializer(portId)};");
                    }
                }

                stringBuilder
                    .Append(baseIndent).Append(INDENT_UNIT).AppendLine("}");

            }

            stringBuilder
                .AppendLine("}");

            return stringBuilder.ToString();
        }


    }
}