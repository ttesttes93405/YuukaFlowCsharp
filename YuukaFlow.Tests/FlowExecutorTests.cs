using YuukaFlow.Core;

namespace YuukaFlow.Tests;

public class FlowExecutorTests
{
    record TestContext
    {
        public int Value { get; set; }

        public TestContext(int value)
        {
            Value = value;
        }
    }


    [Fact]
    public async Task Execute_IsExpected()
    {
        var flowchart = new Flowchart<string, int>(
            entryNodeName: "Start",
            flowNodes: [
                new FlowNode<string, int>("Start",(1, "Middle")),
                new FlowNode<string, int>("Middle",(2, "End")),
                new FlowNode<string, int>("End"),
            ]
        );

        var implementations = new Dictionary<string, FlowNodeImplementation<TestContext, int>>
        {
            ["Start"] = static (context) =>
            {
                context.Value += 1;
                return ValueTask.FromResult(1);
            },
            ["Middle"] = static (context) =>
            {
                context.Value *= 2;
                return ValueTask.FromResult(2);
            },
            ["End"] = static (context) =>
            {
                context.Value -= 1;
                return ValueTask.FromResult(-1);
            },
        };

        var expected = new TestContext(3);

        var executor = new FlowExecutor<TestContext, string, int>(flowchart, implementations);

        var context = new TestContext(1);

        var resultContext = await executor.Execute(context);

        Assert.Equal(expected, resultContext);
    }
}