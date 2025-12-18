using System;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace YuukaFlow.Core
{
    public static class Extensions
    {

        public static int GetCollectionPersistentCode<T>(Collection<T> collection)
        {
            if (collection == null)
                return 0;

            int hash = 17;
            foreach (var item in collection)
            {
                hash = hash * 31 + GetObjectPersistentCode(item);
            }
            return hash;
        }
        public static int GetDictionaryPersistentCode<TKey, TValue>(Dictionary<TKey, TValue>? dict)
        {
            if (dict == null)
                return 0;

            int hash = 17;

            foreach (var pair in dict.OrderBy(kv => kv.Key))
            {
                hash = hash * 31 + GetObjectPersistentCode(pair.Key);
                hash = hash * 31 + GetObjectPersistentCode(pair.Value);
            }
            return hash;
        }
        public static int GetObjectPersistentCode<T>(T obj)
        {
            return obj switch
            {
                null => 0,
                IPersistent persistent => persistent.GetPersistentCode(),
                string ros => GetStringPersistentCode(ros),
                _ => obj.GetHashCode(), // not persistent but best effort
            };
        }
        public static int GetStringPersistentCode(ReadOnlySpan<char> str)
        {
            if (str.Length == 0)
                return 0;

            int hash = 17;
            foreach (var ch in str)
            {
                hash = hash * 31 + ch;
            }
            return hash;
        }
        public static int CombinePersistentCode(params int[] codes)
        {
            int hash = 17;
            foreach (var code in codes)
            {
                if (code == 0)
                    continue;
                hash = hash * 31 + GetObjectPersistentCode(code);
            }
            return hash;
        }


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


                foreach (var pair in flowNode.OutputPorts!)
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