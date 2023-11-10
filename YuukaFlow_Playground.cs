

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YuukaFlow
{
    public static class YuukaFlow_Playground
    {

        class TestState
        {
            public bool IsGood { get; set; }
        }

        enum OutputPort
        {
            Default,
            True,
            False,
            End,
        }

        public static async void Play()
        {
            FlowDocument<string, OutputPort> flowDoc = new()
            {
                EntryNodeName = "Start",
                FlowNodes = new()
                {
                    ["Start"] = new("Start", new()
                    {
                        [OutputPort.True] = "B",
                        [OutputPort.False] = "C",
                    }),
                    ["B"] = new("B", new()
                    {
                        [OutputPort.Default] = "GoodEnding",
                    }),
                    ["C"] = new("C", new()
                    {
                        [OutputPort.Default] = "BadEnding",
                    }),
                    ["GoodEnding"] = new("GoodEnding"),
                    ["BadEnding"] = new("BadEnding"),
                },
            };

            var tasks = new Dictionary<string, Func<TestState, Task<OutputPort>>>()
            {
                ["Start"] = (state) => { state.IsGood = true; return Task.FromResult(OutputPort.True); },
                ["B"] = (state) => Task.FromResult(OutputPort.Default),
                ["C"] = (state) => Task.FromResult(OutputPort.Default),
                ["GoodEnding"] = (state) => Task.FromResult(OutputPort.End),
                ["BadEnding"] = (state) => Task.FromResult(OutputPort.End),
            };


            var executor = new FlowExecutor<TestState, string, OutputPort>(flowDoc, tasks)
            {
                Name = "TestFlow"
            };
            var finalState = await executor.Execute(new TestState());


            Debug.Log(flowDoc.ConvertToCode((name) => $"\"{name}\"", (portId) => $"{nameof(OutputPort)}.{portId}"));

        }
    }
}