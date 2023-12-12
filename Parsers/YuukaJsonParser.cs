

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace YuukaFlow.Parser
{
    public class YuukaJsonParser
    {


        class FlowchartModel
        {
            [JsonProperty("entryNodeName")]
            public string EntryNodeName { get; set; }

            [JsonProperty("flowNodes")]
            public Collection<FlowNodeModel> FlowNodes { get; set; }
        }

        class FlowNodeModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("outputPorts")]
            public Dictionary<string, string> OutputPorts { get; set; }
        }




        public Flowchart<string, string> Deserialize(string json)
        {
            var flowModel = JsonConvert.DeserializeObject<FlowchartModel>(json);

            var flowchart = new Flowchart<string, string>()
            {
                EntryNodeName = flowModel.EntryNodeName,
                FlowNodes = new(flowModel.FlowNodes?
                    .Select(node => new FlowNode<string, string>(
                            node.Name,
                            node.OutputPorts
                        ))
                    .ToList()),
            };

            return flowchart;
        }

        public string Serialize(Flowchart<string, string> flowchart)
        {
            var flowModel = new FlowchartModel()
            {
                EntryNodeName = flowchart.EntryNodeName,
                FlowNodes = new(flowchart.FlowNodes?
                    .Select(node => new FlowNodeModel()
                    {
                        Name = node.Name,
                        OutputPorts = node.OutputPorts
                    })
                    .ToList()),
            };

            var json = JsonConvert.SerializeObject(flowModel);

            return json;
        }

    }


}