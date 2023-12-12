

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YuukaFlow
{
    public class FlowExecutor<TContext, TName, TPortId> where TContext : class
    {

        readonly Flowchart<TName, TPortId> _flowchart;
        readonly Dictionary<TName, Func<TContext, Task<TPortId>>> _implementations;
        public string Name { get; init; }

        public event Action<FlowNode<TName, TPortId>, TPortId, FlowNode<TName, TPortId>> OnFlowNodeChanged;

        public FlowExecutor(Flowchart<TName, TPortId> flowchart, Dictionary<TName, Func<TContext, Task<TPortId>>> implementations)
        {
            _flowchart = flowchart;
            _implementations = implementations;
        }

        public async Task<TContext> Execute(TContext context)
        {
            var flowNodes = _flowchart.FlowNodes;

            var flowNodeQuery = flowNodes
                .ToDictionary(node => node.Name, node => node);

            var allFlowName = flowNodes.Select(node => node.Name).ToHashSet();
            if (allFlowName.SetEquals(_implementations.Keys) == false)
                throw new Exception($"[YuukaFlow] Flow({Name}) implementation not fullmatch\n{nameof(allFlowName)}=[ {string.Join(", ", allFlowName)} ]\n{nameof(_implementations)}.Keys = [ {string.Join(", ", _implementations.Keys)} ]");

            var currentNode = GetFlowNode(_flowchart.EntryNodeName);
            OnFlowNodeChanged?.Invoke(null, default, currentNode);

            while (true)
            {
                if (_implementations.TryGetValue(currentNode.Name, out var implementation) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) implementation not found for node {currentNode.Name}");

                TPortId outputPortId = default;
                try
                {
                    outputPortId = await implementation(context);
                }
                catch (Exception e)
                {
                    throw new Exception($"[YuukaFlow] Flow({Name}) implementation error for node {currentNode.Name}", e);
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

            return context;

            FlowNode<TName, TPortId> GetFlowNode(TName name)
            {
                if (flowNodeQuery.TryGetValue(name, out var node) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) node {name} not found");

                return node;
            }
        }

    }




}