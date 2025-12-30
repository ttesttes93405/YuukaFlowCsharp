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

            if (_flowchart.EntryNodeName == null)
                throw new NullReferenceException(nameof(_flowchart.EntryNodeName));

            ValidateAllNodesHaveImplementations(_flowchart, _implementations);

            var flowNodeQuery = flowNodes
                .ToDictionary(node => node.Name, node => node);

            var currentNode = GetFlowNode(_flowchart.EntryNodeName);

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

                    if (outputPortId == null)
                        throw new Exception($"[YuukaFlow] Flow({Name}) Output port is null for node {currentNode.Name}");

                    if (currentNode.TryGetNextNodeName(outputPortId, out var nextNodeName) == false)
                        throw new Exception($"[YuukaFlow] Flow({Name}) Output port {outputPortId} not found for node {currentNode.Name}, output ports: {string.Join(", ", currentNode.OutputPorts.Keys)}");

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

                if (node == null)
                    throw new Exception($"[YuukaFlow] Flow({Name}) node {name} is null");

                return node;
            }


            static void ValidateAllNodesHaveImplementations(Flowchart<TName, TPortId> _flowchart, Dictionary<TName, FlowNodeImplementation<TContext, TPortId>> _implementations)
            {
                var allFlowName = _flowchart.FlowNodes.Select(node => node.Name).ToHashSet();
                var allImplName = _implementations.Keys.ToHashSet();

                if (allFlowName.SetEquals(allImplName) == true)
                    return;

                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[YuukaFlow] Flow({_flowchart.Name}) implementation mismatch detected:");

                var flowNameExcept = allFlowName.Except(allImplName);
                if (flowNameExcept.Any())
                {
                    sb.AppendLine($"  - Missing implementations for nodes: {string.Join(", ", flowNameExcept)}");
                }

                var implNameExcept = allImplName.Except(allFlowName);
                if (implNameExcept.Any())
                {
                    sb.AppendLine($"  - Extra implementations for non-existing nodes: {string.Join(", ", implNameExcept)}");
                }

                var allFlowNameList = string.Join(", ", _flowchart.FlowNodes.Select(node => node.Name).OrderBy(n => n));
                var allImplementationNameList = string.Join(", ", _implementations.Keys.OrderBy(n => n));

                sb.AppendLine($"  - NODES_IN_FLOWCHART = [ {allFlowNameList} ]");
                sb.AppendLine($"  - IMPLEMENTATIONS    = [ {allImplementationNameList} ]");

                throw new Exception(sb.ToString());
            }
        }

    }




}