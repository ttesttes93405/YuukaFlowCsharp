

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YuukaFlow
{
    public static class YuukaFlow_Playground
    {

        class TestContext
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

        [UnityEditor.MenuItem("YuukaFlow/Playground")]
        public static async void Play()
        {
            Flowchart<string, OutputPort> flowchart = new()
            {
                EntryNodeName = "Start",
                FlowNodes = new()
                {
                    new("Start", new()
                    {
                        [OutputPort.True] = "B",
                        [OutputPort.False] = "C",
                    }),
                    new("B", new()
                    {
                        [OutputPort.Default] = "GoodEnding",
                    }),
                    new("C", new()
                    {
                        [OutputPort.Default] = "BadEnding",
                    }),
                    new("GoodEnding"),
                    new("BadEnding"),
                }
            };

            var Implementations = GetImplementations();


            var executor = new FlowExecutor<TestContext, string, OutputPort>(flowchart, Implementations)
            {
                Name = "TestFlow"
            };
            var finalState = await executor.Execute(new TestContext());


            Debug.Log(flowchart.ConvertToCode((name) => $"\"{name}\"", (portId) => $"{nameof(OutputPort)}.{portId}"));

            Debug.Log(flowchart.GetImplementationCodeTemplate(
                (name) => $"\"{name}\"",
                (portId) => $"{nameof(OutputPort)}.{portId}",
                (name) => $"{name}",
                "string",
                "OutputPort",
                "TestContext"
            ));

            Dictionary<string, Func<TestContext, Task<OutputPort>>> GetImplementations()
            {
                return new()
                {
                    ["Start"] = Start,
                    ["B"] = B,
                    ["C"] = C,
                    ["GoodEnding"] = GoodEnding,
                    ["BadEnding"] = BadEnding,
                };

                async Task<OutputPort> Start(TestContext context)
                {
                    return OutputPort.True;
                    return OutputPort.False;
                }

                async Task<OutputPort> B(TestContext context)
                {
                    return OutputPort.Default;
                }

                async Task<OutputPort> C(TestContext context)
                {
                    return OutputPort.Default;
                }

                async Task<OutputPort> GoodEnding(TestContext context)
                {
                    return default;
                }

                async Task<OutputPort> BadEnding(TestContext context)
                {
                    return default;
                }
            }

        }
    }
}