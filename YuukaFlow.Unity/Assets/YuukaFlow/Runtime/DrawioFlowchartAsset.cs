using YuukaFlow.Core;
using UnityEngine;
using YuukaFlow.Core.Parser;

namespace YuukaFlow.Unity
{
    public class DrawioFlowchartAsset : FlowchartAsset
    {
        [HideInInspector]
        [SerializeField]
        string rawData;

        public override Flowchart<string, string> GetFlowchart()
        {
            var flowchart = DrawioParser.DeserializeFirstDiagram(rawData);
            return flowchart;
        }

        public void SetRawData(string data)
        {
            if (rawData != null)
                throw new System.InvalidOperationException("Raw data has been set already.");

            rawData = data;

            var flowchart = DrawioParser.DeserializeFirstDiagram(rawData);

            UpdatePreview(flowchart);
        }
    }
}