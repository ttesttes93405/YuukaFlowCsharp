using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YuukaFlow.Core
{
    public delegate ValueTask<TPortId> FlowNodeImplementation<in TContext, TPortId>(TContext context);

    public class FlowExecutor<TContext, TName, TPortId> where TContext : class
    {

        readonly Flowchart<TName, TPortId> _flowchart;
        readonly Dictionary<TName, FlowNodeImplementation<TContext, TPortId>> _implementations;

        public string? Name { get; init; }

        public event Action<FlowNode<TName, TPortId>?, TPortId?, FlowNode<TName, TPortId>?>? OnFlowNodeChanged;

        public FlowExecutor(Flowchart<TName, TPortId> flowchart, Dictionary<TName, FlowNodeImplementation<TContext, TPortId>> implementations)
        {
            Name = null;
            OnFlowNodeChanged = null;

            if (flowchart == null)
                throw new NullReferenceException(nameof(flowchart));

            if (implementations == null)
                throw new NullReferenceException(nameof(implementations));

            _flowchart = flowchart;
            _implementations = implementations;
        }

        public async ValueTask<TContext> Execute(TContext context)
        {
            var flowNodes = _flowchart.FlowNodes;

            var flowNodeQuery = flowNodes
                .ToDictionary(node => node.Name, node => node);

            var allFlowName = flowNodes.Select(node => node.Name).ToHashSet();
            if (allFlowName.SetEquals(_implementations.Keys) == false)
                throw new Exception($"[YuukaFlow] Flow({Name}) implementation not full match\n{nameof(allFlowName)}=[ {string.Join(", ", allFlowName)} ]\n{nameof(_implementations)}.Keys = [ {string.Join(", ", _implementations.Keys)} ]");

            if (_flowchart.EntryNodeName == null)
                throw new NullReferenceException(nameof(_flowchart.EntryNodeName));

            var currentNode = GetFlowNode(_flowchart.EntryNodeName);
            if (currentNode == null)
                throw new Exception($"[YuukaFlow] Flow({Name}) entry node {_flowchart.EntryNodeName} not found");

            OnFlowNodeChanged?.Invoke(null, default, currentNode);

            while (true)
            {
                if (_implementations.TryGetValue(currentNode.Name, out var implementation) == false)
                    throw new Exception($"[YuukaFlow] Flow({Name}) implementation not found for node {currentNode.Name}");

                if (implementation == null)
                    throw new Exception($"[YuukaFlow] Flow({Name}) implementation is null for node {currentNode.Name}");

                try
                {
                    TPortId? outputPortId = await implementation(context);
                    if (currentNode.HasOutputPorts == false)
                    {
                        break;
                    }

                    if (currentNode.TryGetNextNodeName(outputPortId, out var nextNodeName) == false)
                        throw new Exception($"[YuukaFlow] Flow({Name}) Output port {outputPortId} not found for node {currentNode.Name}");

                    var prevNode = currentNode;
                    currentNode = GetFlowNode(nextNodeName);

                    OnFlowNodeChanged?.Invoke(prevNode, outputPortId, currentNode);
                }
                catch (Exception e)
                {
                    throw new Exception($"[YuukaFlow] Flow({Name}) implementation error for node {currentNode.Name}", e);
                }
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