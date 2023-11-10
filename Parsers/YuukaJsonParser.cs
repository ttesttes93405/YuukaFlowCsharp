

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace YuukaFlow.Parser
{
    public class YuukaJsonParser
    {


        class FlowDocumentModel
        {
            [JsonProperty("entryNodeName")]
            public string EntryNodeName { get; set; }

            [JsonProperty("flowNodes")]
            public Dictionary<string, FlowNodeModel> FlowNodes { get; set; }
        }

        class FlowNodeModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("outputPorts")]
            public Dictionary<string, string> OutputPorts { get; set; }
        }




        public FlowDocument<string, string> Deserialize(string json)
        {
            var flowModel = JsonConvert.DeserializeObject<FlowDocumentModel>(json);

            var flowDocument = new FlowDocument<string, string>()
            {
                EntryNodeName = flowModel.EntryNodeName,
                FlowNodes = flowModel.FlowNodes?
                    .Select(node => (
                        key: node.Key,
                        value: new FlowNode<string, string>(
                            node.Value.Name,
                            node.Value.OutputPorts
                        )))
                    .ToDictionary(v => v.key, v => v.value),
            };

            return flowDocument;
        }

        public string Serialize(FlowDocument<string, string> flowDoc)
        {
            var flowModel = new FlowDocumentModel()
            {
                EntryNodeName = flowDoc.EntryNodeName,
                FlowNodes = flowDoc.FlowNodes?
                    .Select(node => (
                        key: node.Key,
                        value: new FlowNodeModel()
                        {
                            Name = node.Value.Name,
                            OutputPorts = node.Value.OutputPorts
                        }))
                    .ToDictionary(v => v.key, v => v.value),
            };

            var json = JsonConvert.SerializeObject(flowModel);

            return json;
        }

    }


}