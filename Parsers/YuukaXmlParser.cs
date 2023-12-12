using System;
using System.Xml;
using System.Linq;
using System.Collections.ObjectModel;

namespace YuukaFlow.Parser
{

    public class YuukaXmlParser
    {

        public Flowchart<string, string> Parse(string xmlText)
        {
            XmlDocument XmlDoc = new();
            XmlDoc.LoadXml(xmlText);

            var root = XmlDoc.DocumentElement;

            string entryNodeName = root.SelectSingleNode("flow-entry-node").Attributes["name"].Value;

            var flowXmlNodes = root
                .SelectSingleNode("flow-nodes")
                .SelectNodes("flow-node")
                .Cast<XmlNode>();

            var flowNodes = new Collection<FlowNode<string, string>>(flowXmlNodes
                .Select(xmlNode =>
                {
                    var outputPorts = xmlNode
                        .SelectNodes("output-port")
                        .Cast<XmlNode>()
                        .Select(outputPortXmlNode =>
                        {
                            string portName = outputPortXmlNode.Attributes["name"]?.Value ?? string.Empty;
                            string toName = outputPortXmlNode.Attributes["to"].Value;
                            return (portName, toName);
                        })
                        .ToDictionary(t => t.portName, t => t.toName);

                    var nodeName = xmlNode.Attributes["name"].Value;

                    var result = new FlowNode<string, string>(nodeName)
                    {
                        OutputPorts = outputPorts
                    };

                    return result;
                })
                .ToList());

            var entryMNode = flowNodes.First(node => node.Name == entryNodeName);

            return new()
            {
                EntryNodeName = entryNodeName,
                FlowNodes = flowNodes
            };


        }
    }
}