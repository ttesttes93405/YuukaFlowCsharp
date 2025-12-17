using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace YuukaFlow.Core.Parser
{
    public class DrawioParser
    {

        const string NODE_NAME_ATTRIBUTE = "node-name";
        const string PORT_NAME_ATTRIBUTE = "port-name";
        const string ENTRY_NODE_ATTRIBUTE = "entry-node";

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

        public static Flowchart<TName, TPortId> Deserialize<TName, TPortId>(string xmlText, Func<string, TName> nameHandler, Func<string, TPortId> portIdHandler)
        {
            if (xmlText == null)
                throw new ArgumentNullException(nameof(xmlText));

            if (nameHandler == null)
                throw new ArgumentNullException(nameof(nameHandler));

            if (portIdHandler == null)
                throw new ArgumentNullException(nameof(portIdHandler));

            XmlDocument XmlDoc = new();
            XmlDoc.LoadXml(xmlText);

            var root = XmlDoc.DocumentElement;

            var diagrams = root.SelectNodes("diagram");

            if (diagrams.Count == 0)
                throw new Exception("[YuukaFlow] DrawioParser Deserialize: No diagram found in drawio xml");

            if (diagrams.Count > 1)
                throw new Exception("[YuukaFlow] DrawioParser Deserialize: Multiple diagrams found in drawio xml, only one diagram is supported");

            var diagram = diagrams[0];

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
                    nodes.Add(nodeId, new Node<TName>(nodeId, nameKey));

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

            HashSet<string> includingNodeIds = Enumerable
                .Concat(
                    edges.Select(edge => edge.SourceId),
                    edges.Select(edge => edge.TargetId)
                )
                .ToHashSet();

            var flowNodes = new Collection<FlowNode<TName, TPortId>>(nodes.Values
                .Where(node => includingNodeIds.Contains(node.Id))
                .Select(node =>
                {
                    var outputPorts = edges
                        .Where(edge => edge.SourceId == node.Id)
                        .Where(edge => nodes.ContainsKey(edge.TargetId))
                        .ToDictionary(
                            edge => edge.PortId,
                            edge => nodes[edge.TargetId].Name
                        );

                    var result = new FlowNode<TName, TPortId>(node.Name)
                    {
                        OutputPorts = outputPorts
                    };

                    return result;
                })
                .ToList());

            return new Flowchart<TName, TPortId>(
                entryNodeName: entryNode.Value.Name,
                flowNodes: flowNodes
            );
        }

        public static Flowchart<string, string> Deserialize(string xmlText)
        {
            return Deserialize<string, string>(
                xmlText,
                nameHandler: StringToString,
                portIdHandler: StringToString
            );

            static string StringToString(string str) => str;
        }

    }


}