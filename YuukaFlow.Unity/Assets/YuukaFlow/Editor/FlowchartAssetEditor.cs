using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YuukaFlow.Unity
{
    [CustomEditor(typeof(FlowchartAsset), editorForChildClasses: true)]
    public class FlowchartAssetEditor : Editor
    {
        Texture2D icon;

        public override VisualElement CreateInspectorGUI()
        {
            if (target == null)
                return base.CreateInspectorGUI();

            var flowchartAsset = target as FlowchartAsset;

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;

            if (icon == null)
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GUIDs.YuukaFlowLogo));
    
            var flowchartName = flowchartAsset.FlowchartName;
            var entryNode = flowchartAsset.PreviewEntryNode;
            var nodeCount = flowchartAsset.PreviewNodes.Length;
            var nodes = flowchartAsset.PreviewNodes;

            container.Add(
                CreateTitle("Yuuka Flowchart Asset", icon),
                CreateTextField("Name", flowchartName),
                CreateField("Entry Node", CreateFlowNode(entryNode, icon)),
                CreateField($"Nodes ({nodeCount})", null),
                CreateFlowNodeList(nodes, icon)
            );

            return container;


            static VisualElement CreateTitle(string text, Texture2D icon)
            {
                var titleContainer = new VisualElement();
                titleContainer.style.flexDirection = FlexDirection.Row;
                titleContainer.style.alignItems = Align.Center;
                titleContainer.style.marginBottom = 8;
                titleContainer.style.minHeight = 24;
                titleContainer.style.textOverflow = TextOverflow.Ellipsis;

                if (icon != null)
                {
                    var iconElement = new Image
                    {
                        image = icon
                    };
                    iconElement.style.width = 24;
                    iconElement.style.height = 24;
                    iconElement.style.marginRight = 2;
                    titleContainer.Add(iconElement);
                }

                var title = new Label(text);
                title.style.unityFontStyleAndWeight = FontStyle.Bold;
                title.style.fontSize = 14;
                title
                    .SetElementMargin(16, 2, 16, 2)
                    .AddTo(titleContainer);

                return titleContainer;
            }

            static VisualElement CreateTextField(string label, string value)
            {
                var lbl = new Label(value);
                lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                return CreateField(label, lbl);
            }

            static VisualElement CreateField(string label, VisualElement valueElement)
            {
                var fieldContainer = new VisualElement();
                fieldContainer.style.flexDirection = FlexDirection.Row;
                fieldContainer.SetElementMargin(0, 2, 4, 2);
                fieldContainer.style.minHeight = 20;

                var lbl = new Label(label);
                lbl.SetEnabled(false);
                lbl.style.width = 120;
                lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                fieldContainer.Add(lbl);

                if (valueElement != null)
                    fieldContainer.Add(valueElement);

                return fieldContainer;
            }

            static VisualElement CreateFlowNode(FlowchartAsset.PreviewNode node, Texture2D icon)
            {
                var nodeContainer = new VisualElement();
                nodeContainer.style.flexDirection = FlexDirection.Row;
                nodeContainer.style.backgroundColor = new Color(1, 1, 1, 0.1F);
                nodeContainer.style.flexGrow = 0;
                nodeContainer.style.minHeight = 20;
                nodeContainer
                    .SetElementPadding(0, 6, 0, 4)
                    .SetElementBorderRadius(8)
                    .SetElementBorderWidth(1)
                    .SetElementBorderColor(new Color(0, 0, 0, 0.4F));

                var iconElement = new Image
                {
                    image = icon
                };
                iconElement.style.width = 18;
                iconElement.style.height = 18;
                nodeContainer.Add(iconElement);

                var val = new Label(node.nodeName);
                val.style.unityTextAlign = TextAnchor.MiddleLeft;
                val.style.textOverflow = TextOverflow.Ellipsis;
                nodeContainer.Add(val);

                return nodeContainer;
            }

            static VisualElement CreateFlowNodeList(FlowchartAsset.PreviewNode[] nodes, Texture2D icon)
            {
                var listContainer = new VisualElement();
                listContainer.style.flexDirection = FlexDirection.Column;
                listContainer.style.alignItems = Align.FlexStart;
                listContainer.style.marginTop = 6;

                foreach (var node in nodes)
                {
                    var val = CreateFlowNode(node, icon);
                    val.style.marginBottom = 4;
                    listContainer.Add(val);
                }

                return listContainer;
            }


        }


    }
}