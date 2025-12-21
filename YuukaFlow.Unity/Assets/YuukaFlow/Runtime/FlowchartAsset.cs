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
            public string nodeName;
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
                previewEntryNode = new PreviewNode { nodeName = flowchart.EntryNodeName };
                previewNodes = flowchart.FlowNodes.Select(n => new PreviewNode { nodeName = n.Name }).ToArray();
            }
        }

        public abstract Flowchart<string, string> GetFlowchart();
    }
}