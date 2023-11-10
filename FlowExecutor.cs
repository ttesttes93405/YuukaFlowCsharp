

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YuukaFlow
{
    public class FlowExecutor<TState, TName, TPortId> where TState : class
    {

        readonly FlowDocument<TName, TPortId> _flowDoc;
        readonly Dictionary<TName, Func<TState, Task<TPortId>>> _activities;
        public string Name { get; init; }

        public event Action< FlowNode<TName, TPortId>, TPortId, FlowNode<TName, TPortId>> OnFlowNodeChanged;

        public FlowExecutor(FlowDocument<TName, TPortId> flowDoc, Dictionary<TName, Func<TState, Task<TPortId>>> activities)
        {
            _flowDoc = flowDoc;
            _activities = activities;
        }

        public async Task<TState> Execute(TState state)
        {
            var flowNodes = _flowDoc.FlowNodes;

            var allFlowName = new HashSet<TName>(flowNodes.Keys);
            if (allFlowName.SetEquals(_activities.Keys) == false)
                throw new Exception($"[YuukaFlow] Flow({Name}) activity not fullmatch\n{nameof(allFlowName)}=[ {string.Join(", ", allFlowName)} ]\n{nameof(_activities)}.Keys = [ {string.Join(", ", _activities.Keys)} ]");

            var currentNode = GetFlowNode(_flowDoc.EntryNodeName);
            OnFlowNodeChanged?.Invoke(null, default, currentNode);

            while (true)
            {
                if (_activities.TryGetValue(currentNode.Name, out var activity) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) activity not found for node {currentNode.Name}");

                TPortId outputPortId = default;
                try
                {
                    outputPortId = await activity(state);
                }
                catch (Exception e)
                {
                    throw new Exception($"[YuukaFlow] Flow({Name}) activity error for node {currentNode.Name}", e);
                }

                if (currentNode.OutputPorts == null || currentNode.OutputPorts.Count == 0)
                {
                    break;
                }

                if (currentNode.OutputPorts.TryGetValue(outputPortId, out var nextNodeName) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) Output port {outputPortId} not found for node {currentNode.Name}");

                var prevNode = currentNode;
                currentNode = GetFlowNode(nextNodeName);
                OnFlowNodeChanged?.Invoke(prevNode, outputPortId, currentNode);
            }

            return state;

            FlowNode<TName, TPortId> GetFlowNode(TName name)
            {
                if (flowNodes.TryGetValue(name, out var node) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) node {name} not found");

                return node;
            }
        }

    }




}