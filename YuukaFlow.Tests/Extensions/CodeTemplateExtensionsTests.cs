namespace YuukaFlow.Tests.Extensions;

using YuukaFlow.Core;
using YuukaFlow.Core.Extensions;

public class CodeTemplateExtensionsTests
{
    [Fact]
    public void GetImplementationCodeTemplate_EmitsFingerprintAttribute_MatchingFlowchartFingerprint()
    {
        var flowchart = new Flowchart<string, string>(
            entryNodeName: "Start",
            flowNodes:
            [
                new FlowNode<string, string>("Start", ("Next", "End")),
                new FlowNode<string, string>("End"),
            ]
        );

        var code = flowchart.GetImplementationCodeTemplate(
            nameSerializer: s => $"\"{s}\"",
            portIdSerializer: s => $"\"{s}\"",
            implementationNameSerializer: s => s,
            nameTypeName: "string",
            portIdTypeName: "string",
            contextTypeName: "Context"
        );

        var expectedAttribute = $"[Fingerprint(\"{flowchart.GetFingerprint().Code:x8}\")]";
        Assert.Contains(expectedAttribute, code);
    }
}
