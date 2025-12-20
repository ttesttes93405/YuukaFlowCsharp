using YuukaFlow.Core;
using UnityEngine;

namespace YuukaFlow.Unity
{
    public abstract class FlowchartAsset : ScriptableObject
    {
        public abstract Flowchart<string, string> GetFlowchart();
    }
}