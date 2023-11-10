using System;
using System.Collections.Generic;
using System.Linq;

namespace YuukaFlow.Parser
{



    public class LucidchartCSVParser
    {
        public record FlowData
        {
            public int Id { get; init; }
            public string Name { get; init; }
            // public string ShapeLibrary { get; init; }
            public int PageID { get; init; }
            // public string ContainedBy { get; init; }
            // public string Group { get; init; }
            public string LineSource { get; init; }
            public string LineDestination { get; init; }
            // public string SourceArrow { get; init; }
            // public string DestinationArrow { get; init; }
            // public string Status { get; init; }
            // public string TextArea1 { get; init; }
            // public string Comments { get; init; }
            public string nodename { get; init; }
            public string portname { get; init; }
            public string pagename { get; init; }
        }
        enum LucidchartNodeType
        {
            TerminatorBlock,
            Decision,
            Process,
            OffPageLink,
            Line,
            Other,
        }

        LucidchartNodeType GetNodeType(string name)
        {
            return name switch
            {
                "Terminator" => LucidchartNodeType.TerminatorBlock,
                "Decision" => LucidchartNodeType.Decision,
                "Process" => LucidchartNodeType.Process,
                "Line" => LucidchartNodeType.Line,
                "Off-page link" => LucidchartNodeType.OffPageLink,
                _ => LucidchartNodeType.Other,
            };

        }


        public Dictionary<string, FlowDocument<string, string>> Parse(string csvContent)
        {
            var flowDatas = ParseCSVToItemList<FlowData>(csvContent);

            var result = flowDatas
                .GroupBy(d => d.PageID)
                .Select(d => (
                    pageId: flowDatas.FirstOrDefault(flowData => flowData.Id == d.Key)?.pagename ?? string.Empty,
                    flowDatas: d.ToList()
                ))
                .ToDictionary(d => d.pageId, d => ConvertListItemToFlowDocument(d.flowDatas))
                .Where((d) => string.IsNullOrEmpty(d.Key) == false) // 去除沒有pagename的元素 包含page本身
                .ToDictionary(d => d.Key, d => d.Value);

            return result;
        }


        FlowDocument<string, string> ConvertListItemToFlowDocument(List<FlowData> flowDatas)
        {

            string entryNodeName = null;
            List<FlowData> nodes = new();
            List<FlowData> lines = new();


            var nodeTypes = flowDatas
                .Select(flowData => (flowData, type: GetNodeType(flowData.Name)))
                .ToList();
            entryNodeName = nodeTypes
                .Where((nodeType) => nodeType.type == LucidchartNodeType.TerminatorBlock)
                .Select((nodeType) => GetNodeName(nodeType.flowData))
                .FirstOrDefault();
            nodes = nodeTypes
                .Where((nodeType) => nodeType.type == LucidchartNodeType.Decision ||
                                     nodeType.type == LucidchartNodeType.Process ||
                                     nodeType.type == LucidchartNodeType.TerminatorBlock ||
                                     nodeType.type == LucidchartNodeType.OffPageLink)
                .Select((nodeType) => nodeType.flowData)
                .ToList();
            lines = nodeTypes
                .Where((nodeType) => nodeType.type == LucidchartNodeType.Line)
                .Select((nodeType) => nodeType.flowData)
                .ToList();

            var flowNodes = nodes
                 .Select(flowData =>
                 {
                     var outputPorts = new Dictionary<string, string>();

                     var ports = lines.Where((lines) => lines.LineSource == flowData.Id.ToString()).ToList();

                     if (ports.Count > 0)
                     {
                         ports.ForEach((port) =>
                         {
                             var value = nodes.First((node) => node.Id.ToString() == port.LineDestination).nodename;
                             var key = port.portname ?? string.Empty;
                             outputPorts.Add(key, value);
                         });
                     }
                     else
                     {
                         outputPorts = null;
                     }
                     return new FlowNode<string, string>(GetNodeName(flowData), outputPorts);
                 })
                 .ToDictionary(node => node.Name, node => node);


            return new FlowDocument<string, string>()
            {
                EntryNodeName = entryNodeName,
                FlowNodes = flowNodes
            };


            string GetNodeName(FlowData flowData)
            {
                if (string.IsNullOrEmpty(flowData.nodename))
                    return flowData.Id.ToString();
                else
                    return flowData.nodename;
            }
        }


        List<T> ParseCSVToItemList<T>(string csvContent) where T : new()
        {
            var lines = SpliteContent(csvContent);
            var headers = TermList(lines[0]).Select(text => text.Replace(" ", "")).ToArray();
            var properties = typeof(T).GetProperties();
            var objects = new List<T>();

            for (int i = 1; i < lines.Count; i++)
            {
                var obj = new T();
                var values = TermList(lines[i]);

                for (int j = 0; j < headers.Length; j++)
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        if (string.IsNullOrEmpty(values[j]) && property.PropertyType.IsValueType)
                        {
                            // 如果值是空字符串且屬性類型是值類型（比如 int），則設置為該值類型的默認值
                            property.SetValue(obj, Activator.CreateInstance(property.PropertyType));
                        }
                        else
                            property.SetValue(obj, Convert.ChangeType(values[j], property.PropertyType));
                    }
                }

                objects.Add(obj);
            }


            return objects;


            static string[] TermList(string[] strings)
            {
                return strings.Select(s => s.Trim()).ToArray();
            }

            static List<string[]> SpliteContent(string csvContent)
            {
                var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var result = new List<string[]>();

                foreach (var line in lines)
                {
                    var fields = new List<string>();
                    var field = new System.Text.StringBuilder();
                    bool inQuotes = false;


                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];

                        if (inQuotes)
                        {
                            if (c == '"' && i < line.Length - 1 && line[i + 1] == '"')
                            {
                                i++;
                            }
                            else if (c == '"')
                            {
                                inQuotes = false;
                            }

                            field.Append(c);
                        }
                        else
                        {
                            if (c == ',')
                            {
                                fields.Add(field.ToString());
                                field.Clear();
                            }
                            else if (c == '"')
                            {
                                inQuotes = true;
                                field.Append(c);
                            }
                            else
                            {
                                field.Append(c);
                            }

                        }


                    }


                    if (field.Length > 0)
                    {
                        fields.Add(field.ToString());
                    }

                    if (line[line.Length - 1] == ',')
                    {
                        fields.Add(field.ToString());
                    }

                    result.Add(fields.ToArray());
                }


                return result;
            }
        }

    }
}