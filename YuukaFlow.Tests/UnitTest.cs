namespace YuukaFlow.Tests;

using YuukaFlow.Core;
using YuukaFlow.Core.Parser;

public class UnitTest
{
    [Fact]
    public void ParseDrawioFlowchartIsExpected()
    {
        const string xml = """
<mxfile host="65bd71144e">
    <diagram id="WW_xL7Uk0EXr_TR8rbgo" name="TestFlow">
        <mxGraphModel dx="1079" dy="529" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="827" pageHeight="1169" math="0" shadow="0">
            <root>
                <mxCell id="0"/>
                <mxCell id="1" parent="0"/>
                <object label="%node-name%" placeholders="1" node-name="Entry" entry-node="1" id="2">
                    <mxCell style="rounded=1;whiteSpace=wrap;html=1;glass=0;editable=1;movable=1;resizable=1;rotatable=1;deletable=1;locked=0;connectable=1;comic=0;enumerate=0;enumerateValue=10;treeFolding=0;" parent="1" vertex="1">
                        <mxGeometry x="340" y="100" width="120" height="60" as="geometry"/>
                    </mxCell>
                </object>
                <object label="%port-name%" port-name="PORT1" placeholders="1" id="3">
                    <mxCell style="html=1;edgeStyle=orthogonalEdgeStyle;exitDx=0;exitDy=0;exitX=0.5;exitY=1;movable=1;resizable=1;rotatable=1;deletable=1;editable=1;locked=0;connectable=1;" parent="1" source="2" target="4" edge="1">
                        <mxGeometry relative="1" as="geometry">
                            <mxPoint x="390" y="200.0000000000001" as="sourcePoint"/>
                            <mxPoint x="400" y="390" as="targetPoint"/>
                        </mxGeometry>
                    </mxCell>
                </object>
                <object label="%node-name%" placeholders="1" node-name="NODE2" id="4">
                    <mxCell style="rounded=1;whiteSpace=wrap;html=1;glass=0;editable=1;movable=1;resizable=1;rotatable=1;deletable=1;locked=0;connectable=1;comic=0;enumerate=0;enumerateValue=10;treeFolding=0;" parent="1" vertex="1">
                        <mxGeometry x="340" y="410" width="120" height="60" as="geometry"/>
                    </mxCell>
                </object>
                <object label="%node-name%" placeholders="1" node-name="NODE3" id="5">
                    <mxCell style="rounded=1;whiteSpace=wrap;html=1;glass=0;editable=1;movable=1;resizable=1;rotatable=1;deletable=1;locked=0;connectable=1;comic=0;enumerate=0;enumerateValue=10;treeFolding=0;" parent="1" vertex="1">
                        <mxGeometry x="340" y="550" width="120" height="60" as="geometry"/>
                    </mxCell>
                </object>
                <object label="%port-name%" port-name="PORT2" placeholders="1" id="6">
                    <mxCell style="html=1;edgeStyle=orthogonalEdgeStyle;exitDx=0;exitDy=0;exitX=1;exitY=0.5;movable=1;resizable=1;rotatable=1;deletable=1;editable=1;locked=0;connectable=1;entryX=1;entryY=0.5;entryDx=0;entryDy=0;" parent="1" source="4" target="5" edge="1">
                        <mxGeometry relative="1" as="geometry">
                            <mxPoint x="410" y="170" as="sourcePoint"/>
                            <mxPoint x="410" y="420" as="targetPoint"/>
                            <Array as="points">
                                <mxPoint x="530" y="440"/>
                                <mxPoint x="530" y="580"/>
                            </Array>
                        </mxGeometry>
                    </mxCell>
                </object>
                <object label="%port-name%" port-name="線段名稱3" placeholders="1" id="7">
                    <mxCell style="html=1;edgeStyle=orthogonalEdgeStyle;exitDx=0;exitDy=0;exitX=0;exitY=0.5;movable=1;resizable=1;rotatable=1;deletable=1;editable=1;locked=0;connectable=1;entryX=0;entryY=0.5;entryDx=0;entryDy=0;" parent="1" source="4" target="2" edge="1">
                        <mxGeometry relative="1" as="geometry">
                            <mxPoint x="470" y="450" as="sourcePoint"/>
                            <mxPoint x="470" y="590" as="targetPoint"/>
                            <Array as="points">
                                <mxPoint x="270" y="440"/>
                                <mxPoint x="270" y="130"/>
                            </Array>
                        </mxGeometry>
                    </mxCell>
                </object>
            </root>
        </mxGraphModel>
    </diagram>
</mxfile>
""";


        var flowchart = DrawioParser.DeserializeFirstDiagram(xml);


        Console.WriteLine($"Node: {flowchart.Name} {flowchart.GetPersistentCode()}");
        foreach (var node in flowchart.FlowNodes)
        {
            Console.WriteLine($"Node: {node.Name} {node.GetPersistentCode()}");
        }


        Assert.Equal(69981277, flowchart.GetPersistentCode());
    }



}