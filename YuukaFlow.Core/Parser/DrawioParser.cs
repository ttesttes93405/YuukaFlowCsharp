using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace YuukaFlow.Core.Parser
{
    public class DrawioParser
    {
        const string ENTRY_NODE_ATTRIBUTE = "entry-node";
        const string NODE_NAME_ATTRIBUTE = "node-name";
        const string PORT_NAME_ATTRIBUTE = "port-name";


        readonly struct Node<TName>
        {
            public string Id { get; init; }
            public TName Name { get; init; }
            public Node(string id, TName name)
            {
                Id = id;
                Name = name;
            }
        }

        readonly struct Edge<TPortId>
        {
            public TPortId PortId { get; init; }
            public string SourceId { get; init; }
            public string TargetId { get; init; }
            public Edge(TPortId portId, string sourceId, string targetId)
            {
                PortId = portId;
                SourceId = sourceId;
                TargetId = targetId;
            }
        }


        public static Flowchart<TName, TPortId>[] Deserialize<TName, TPortId>(string xmlText, Func<string, TName> nameDeserializer, Func<string, TPortId> portIdDeserializer)
        {
            if (xmlText == null)
                throw new ArgumentNullException(nameof(xmlText));

            if (nameDeserializer == null)
                throw new ArgumentNullException(nameof(nameDeserializer));

            if (portIdDeserializer == null)
                throw new ArgumentNullException(nameof(portIdDeserializer));

            XmlDocument XmlDoc = new();
            XmlDoc.LoadXml(xmlText);

            var root = XmlDoc.DocumentElement;

            return root
                .SelectNodes("diagram")
                .Cast<XmlElement>()
                .Select(diagram => ParseDiagramElement<TName, TPortId>(diagram, nameDeserializer, portIdDeserializer))
                .ToArray();
        }

        public static Flowchart<TName, TPortId> DeserializeFirstDiagram<TName, TPortId>(string xmlText, Func<string, TName> nameDeserializer, Func<string, TPortId> portIdDeserializer)
        {
            if (xmlText == null)
                throw new ArgumentNullException(nameof(xmlText));

            if (nameDeserializer == null)
                throw new ArgumentNullException(nameof(nameDeserializer));

            if (portIdDeserializer == null)
                throw new ArgumentNullException(nameof(portIdDeserializer));

            XmlDocument XmlDoc = new();
            XmlDoc.LoadXml(xmlText);

            var root = XmlDoc.DocumentElement;

            if (root.SelectSingleNode("diagram") is XmlElement diagram == false)
                throw new Exception("[YuukaFlow] DrawioParser DeserializeFirstDiagram: No diagram element found in drawio xml");

            return ParseDiagramElement<TName, TPortId>(diagram, nameDeserializer, portIdDeserializer);
        }

        public static Flowchart<TName, TPortId> ParseDiagramElement_origin<TName, TPortId>(XmlElement diagram, Func<string, TName> nameHandler, Func<string, TPortId> portIdHandler)
        {
            if (diagram == null)
                throw new ArgumentNullException(nameof(diagram));

            if (nameHandler == null)
                throw new ArgumentNullException(nameof(nameHandler));

            if (portIdHandler == null)
                throw new ArgumentNullException(nameof(portIdHandler));

            string flowchartName = "";
            if (diagram.HasAttribute("name"))
            {
                flowchartName = diagram.GetAttribute("name");
            }

            var diagramRoot = diagram["mxGraphModel"]["root"];
            var diagramObjects = diagramRoot.SelectNodes("object").Cast<XmlElement>();

            Dictionary<string, Node<TName>> nodes = new();
            List<Edge<TPortId>> edges = new();
            Node<TName>? entryNode = null;

            foreach (var obj in diagramObjects)
            {
                bool isNode = obj.HasAttribute(NODE_NAME_ATTRIBUTE);
                if (isNode)
                {
                    string nodeName = obj.Attributes[NODE_NAME_ATTRIBUTE].Value;
                    string? nodeId = obj.Attributes["id"]?.Value;
                    if (nodeId == null)
                        continue;

                    TName nameKey = nameHandler(nodeName);
                    if (nodes.TryAdd(nodeId, new Node<TName>(nodeId, nameKey)) == false)
                        throw new Exception($"[YuukaFlow] DrawioParser Deserialize: Duplicate node id '{nodeId}' found in drawio xml");

                    bool isEntryNode = obj.HasAttribute(ENTRY_NODE_ATTRIBUTE);
                    if (isEntryNode)
                    {
                        if (entryNode != null)
                            throw new Exception("[YuukaFlow] DrawioParser Deserialize: Multiple entry nodes found in drawio xml");
                        entryNode = new Node<TName>(nodeId, nameKey);
                    }
                    continue;
                }

                bool isEdge = obj.HasAttribute(PORT_NAME_ATTRIBUTE);
                if (isEdge)
                {
                    string portId = obj.Attributes[PORT_NAME_ATTRIBUTE].Value;
                    string? nodeId = obj.Attributes["id"]?.Value;
                    string? sourceId = obj["mxCell"].Attributes["source"]?.Value;
                    string? targetId = obj["mxCell"].Attributes["target"]?.Value;
                    if (nodeId == null || sourceId == null || targetId == null)
                        continue;

                    TPortId portIdKey = portIdHandler(portId);
                    edges.Add(new Edge<TPortId>(portIdKey, sourceId, targetId));
                    continue;
                }
            }

            if (entryNode.HasValue == false)
                throw new Exception("[YuukaFlow] DrawioParser Deserialize: No entry node found in drawio xml");

            bool hasDuplicateNodeNames = nodes.Values
                .GroupBy(node => node.Name)
                .Any(group => group.Count() > 1);
            if (hasDuplicateNodeNames)
            {
                var names = nodes.Values.GroupBy(node => node.Name).Where(g => g.Count() > 1).Select(g => g.Key);
                throw new Exception($"[YuukaFlow] DrawioParser Deserialize: Duplicate node names found in drawio xml. Node names: {string.Join(", ", names)}");
            }
            HashSet<string> includingNodeIds = Enumerable
                .Concat(
                    edges.Select(edge => edge.SourceId),
                    edges.Select(edge => edge.TargetId)
                )
                .ToHashSet();

            var flowNodes = nodes.Values
                .Where(node => includingNodeIds.Contains(node.Id))
                .Select(node =>
                {
                    var outputPorts = edges
                        .Where(edge => edge.SourceId == node.Id)
                        .Where(edge => nodes.ContainsKey(edge.TargetId))
                        .Select(edge => (portId: edge.PortId, targetNode: nodes[edge.TargetId].Name))
                        .ToArray();

                    return new FlowNode<TName, TPortId>(node.Name, outputPorts);
                })
                .ToArray();

            return new Flowchart<TName, TPortId>(
                entryNodeName: entryNode.Value.Name,
                flowNodes: flowNodes
            )
            {
                Name = flowchartName,
            };
        }

        public static Flowchart<TName, TPortId> ParseDiagramElement<TName, TPortId>(XmlElement diagram, Func<string, TName> nameHandler, Func<string, TPortId> portIdHandler)
        {
            if (diagram == null)
                throw new ArgumentNullException(nameof(diagram));

            if (nameHandler == null)
                throw new ArgumentNullException(nameof(nameHandler));

            if (portIdHandler == null)
                throw new ArgumentNullException(nameof(portIdHandler));

            string flowchartName = "";
            if (diagram.HasAttribute("name"))
            {
                flowchartName = diagram.GetAttribute("name");
            }

            var diagramRoot = diagram["mxGraphModel"]["root"];
            var diagramObjects = diagramRoot.SelectNodes("object").Cast<XmlElement>();

            FlowchartBuilder<TName, TPortId> builder = new(flowchartName);


            var nodeIdQuery = new Dictionary<string, TName>();
            var nodes = diagramObjects.Where(obj => obj.HasAttribute(NODE_NAME_ATTRIBUTE));
            foreach (var node in nodes)
            {
                string nodeName = node.Attributes[NODE_NAME_ATTRIBUTE].Value;
                string? nodeId = node.Attributes["id"]?.Value;
                if (nodeId == null)
                    continue;

                TName nameKey = nameHandler(nodeName);
                builder.AddNode(nameKey);
                nodeIdQuery[nodeId] = nameKey;

                bool isEntryNode = node.HasAttribute(ENTRY_NODE_ATTRIBUTE);
                if (isEntryNode)
                {
                    builder.SetEntryNode(nameKey);
                }
            }

            var edges = diagramObjects.Where(obj => obj.HasAttribute(PORT_NAME_ATTRIBUTE));
            foreach (var edge in edges)
            {
                string portId = edge.Attributes[PORT_NAME_ATTRIBUTE].Value;
                string? nodeId = edge.Attributes["id"]?.Value;
                var cell = edge["mxCell"];
                string? sourceId = cell.Attributes["source"]?.Value;
                string? targetId = cell.Attributes["target"]?.Value;
                if (nodeId == null || sourceId == null || targetId == null)
                    continue;

                TPortId portIdKey = portIdHandler(portId);

                if (nodeIdQuery.TryGetValue(sourceId, out var sourceNode) == false)
                    throw new Exception($"[YuukaFlow] DrawioParser ParseDiagramElement: Source node id '{sourceId}' not found for edge in drawio xml");

                if (nodeIdQuery.TryGetValue(targetId, out var targetNode) == false)
                    throw new Exception($"[YuukaFlow] DrawioParser ParseDiagramElement: Target node id '{targetId}' not found for edge in drawio xml");

                builder.AddConnection(sourceNode, portIdKey, targetNode);
            }

            return builder.Build();
        }

        public static Flowchart<string, string> DeserializeFirstDiagram(string xmlText)
        {
            return DeserializeFirstDiagram<string, string>(
                xmlText,
                nameDeserializer: StringToString,
                portIdDeserializer: StringToString
            );

            static string StringToString(string str) => str;
        }

    }


}