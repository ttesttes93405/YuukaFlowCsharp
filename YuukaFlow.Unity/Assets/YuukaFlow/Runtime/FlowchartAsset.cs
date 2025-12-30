using YuukaFlow.Core;
using UnityEngine;
using System.Linq;
using System;

namespace YuukaFlow.Unity
{
    public abstract class FlowchartAsset : ScriptableObject
    {
        [Serializable]
        public struct PreviewNode
        {
            public string NodeName;
            public bool HasOutPort;

            public static PreviewNode FromFlowNode(FlowNode<string, string> node)
            {
                return new PreviewNode
                {
                    NodeName = node.Name,
                    HasOutPort = node.HasOutputPorts,
                };
            }
        }

        [SerializeField]
        protected string flowchartName;

        [SerializeField]
        protected PreviewNode previewEntryNode;

        [SerializeField]
        protected PreviewNode[] previewNodes = new PreviewNode[0];

        public string FlowchartName => flowchartName;
        public PreviewNode PreviewEntryNode => previewEntryNode;
        public PreviewNode[] PreviewNodes => previewNodes;

        public void UpdatePreview(Flowchart<string, string> flowchart)
        {
            if (flowchart == null)
            {
                flowchartName = default;
                previewEntryNode = default;
                previewNodes = new PreviewNode[0];
            }
            else
            {
                flowchartName = flowchart.Name;
                previewEntryNode = new PreviewNode { NodeName = flowchart.EntryNodeName };
                previewNodes = flowchart.FlowNodes.Select(PreviewNode.FromFlowNode).ToArray();
            }
        }

        public abstract Flowchart<string, string> GetFlowchart();

        public Flowchart<TName, TPortId> GetFlowchart<TName, TPortId>(Func<string, TName> nameConverter, Func<string, TPortId> portIdConverter)
        {
            var baseFlowchart = GetFlowchart();
            try
            {
                var flowchart = baseFlowchart.ConvertTypes(nameConverter, portIdConverter);
                return flowchart;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to convert flowchart types.", e);
            }
        }
    }
}