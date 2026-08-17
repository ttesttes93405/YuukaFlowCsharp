using System;
using System.Collections.Generic;

namespace YuukaFlow.Core
{
    public class FlowchartBuilder<TName, TPortId>
    {
        bool isEntryNodeNodeSet = false;

        private TName? entryNodeName;
        readonly List<TName> nodeNames = new();
        readonly HashSet<TName> nodeNameSet = new();
        readonly HashSet<(TName from, TPortId portId, TName to)> connections = new();
        readonly string flowchartName;

        bool isBuilt = false;

        public FlowchartBuilder(string flowchartName)
        {
            this.flowchartName = flowchartName;
            entryNodeName = default;
        }

        public FlowchartBuilder<TName, TPortId> SetEntryNode(TName entryNodeName)
        {
            ThrowIfBuilt();

            this.entryNodeName = entryNodeName;
            isEntryNodeNodeSet = true;
            return this;
        }

        public FlowchartBuilder<TName, TPortId> AddNode(TName nodeName)
        {
            ThrowIfBuilt();

            if (nodeNameSet.Contains(nodeName))
                throw new InvalidOperationException($"Node with name {nodeName} already exists in the flowchart.");
            nodeNameSet.Add(nodeName);
            nodeNames.Add(nodeName);
            return this;
        }

        public FlowchartBuilder<TName, TPortId> AddConnection(TName fromNode, TPortId portId, TName toNode)
        {
            ThrowIfBuilt();

            var connection = (from: fromNode, portId: portId, to: toNode);
            if (connections.Contains(connection))
                throw new InvalidOperationException($"Connection from {fromNode} via port {portId} to {toNode} already exists.");

            connections.Add(connection);
            return this;
        }

        public Flowchart<TName, TPortId> Build()
        {
            if (isEntryNodeNodeSet == false)
                throw new InvalidOperationException("Entry node must be set before building the flowchart.");

            if (entryNodeName == null)
                throw new InvalidOperationException("Entry node name must be set before building the flowchart.");

            var flowNodes = new List<FlowNode<TName, TPortId>>();
            foreach (var nodeName in nodeNames)
            {
                var outgoingConnections = new List<(TPortId portId, TName toNode)>();
                foreach (var (from, portId, to) in connections)
                {
                    if (from != null && to != null && from.Equals(nodeName))
                    {
                        outgoingConnections.Add((portId, to));
                    }
                }
                var flowNode = new FlowNode<TName, TPortId>(nodeName, outgoingConnections.ToArray());
                flowNodes.Add(flowNode);
            }

            isBuilt = true;

            return new Flowchart<TName, TPortId>(
                entryNodeName: entryNodeName,
                flowNodes: flowNodes.ToArray()
            )
            {
                Name = flowchartName,
            };
        }

        void ThrowIfBuilt()
        {
            if (isBuilt)
                throw new InvalidOperationException("Flowchart has already been built and cannot be modified.");
        }
    }
}