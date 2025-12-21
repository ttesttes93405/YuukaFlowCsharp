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
        Texture2D entryNodeIcon;

        public override VisualElement CreateInspectorGUI()
        {
            if (target == null)
                return base.CreateInspectorGUI();

            var flowchartAsset = target as FlowchartAsset;

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;

            if (icon == null)
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GUIDs.YuukaFlowLogo));

            if (entryNodeIcon == null)
                entryNodeIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GUIDs.EntryNodeIcon));

            var flowchartName = flowchartAsset.FlowchartName;
            var entryNode = flowchartAsset.PreviewEntryNode;
            var nodeCount = flowchartAsset.PreviewNodes.Length;
            var nodes = flowchartAsset.PreviewNodes;

            var entryNodeIconElement = CreateIcon(entryNodeIcon, 12, 16, new Color(52 / 255f, 207 / 255f, 119 / 255f));

            container.Add(
                // CreateTitle("Yuuka Flowchart Asset", icon),
                // CreateTextField("Name", flowchartName),
                CreateTitle(flowchartName, icon),
                // CreateTextField("Name", flowchartName),
                // CreateField("Entry Node", CreateFlowNode(entryNode, icon)),
                CreateField($"Nodes ({nodeCount})", CreateFlowNodeList(nodes, entryNode.NodeName, entryNodeIconElement))
            );

            return container;


            static VisualElement CreateTitle(string text, Texture2D icon)
            {
                var titleContainer = new VisualElement();
                titleContainer.style.flexDirection = FlexDirection.Row;
                titleContainer.style.alignItems = Align.Center;
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
                    iconElement.style.marginRight = 4;
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

            static VisualElement CreateField(string label, VisualElement valueElement)
            {
                var fieldContainer = new VisualElement();
                fieldContainer.style.flexDirection = FlexDirection.Column;
                fieldContainer.SetElementMargin(4, 2, 12, 2);
                fieldContainer.style.alignItems = Align.FlexStart;

                var lbl = new Label(label);
                lbl.SetEnabled(false);
                lbl.style.fontSize = 10;
                lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                lbl.style.marginBottom = 4;
                fieldContainer.Add(lbl);

                if (valueElement != null)
                {
                    fieldContainer.Add(valueElement);
                }

                return fieldContainer;
            }

            static VisualElement CreateFlowNode(FlowchartAsset.PreviewNode node, VisualElement icon)
            {
                var nodeContainer = new VisualElement();
                nodeContainer.style.flexDirection = FlexDirection.Row;
                nodeContainer.style.backgroundColor = new Color(1, 1, 1, 0.1F);
                nodeContainer.style.flexGrow = 0;
                nodeContainer.style.minHeight = 20;
                nodeContainer.style.alignItems = Align.Center;
                nodeContainer
                    .SetElementPadding(0, 6, 0, 6)
                    .SetElementBorderRadius(8)
                    .SetElementBorderWidth(1)
                    .SetElementBorderColor(new Color(0, 0, 0, 0.4F));

                if (icon != null)
                {
                    icon.AddTo(nodeContainer);
                }

                var val = new Label(node.NodeName);
                val.style.unityTextAlign = TextAnchor.MiddleLeft;
                val.style.textOverflow = TextOverflow.Ellipsis;
                nodeContainer.Add(val);

                return nodeContainer;
            }

            static VisualElement CreateFlowNodeList(FlowchartAsset.PreviewNode[] nodes, string entryNodeName, VisualElement icon)
            {
                var listContainer = new VisualElement();
                listContainer.style.flexDirection = FlexDirection.Column;
                listContainer.style.alignItems = Align.FlexStart;
                // listContainer.style.marginTop = 6;

                foreach (var node in nodes)
                {
                    bool isEntryNode = node.NodeName == entryNodeName;
                    var val = CreateFlowNode(node, isEntryNode ? icon : null);
                    val.style.marginBottom = 4;
                    listContainer.Add(val);
                }

                return listContainer;
            }


            static VisualElement CreateIcon(Texture2D icon, int size, int frameSize, Color color)
            {
                var iconElement = new VisualElement();
                iconElement.style.width = size;
                iconElement.style.height = size;
                iconElement.style.backgroundImage = new StyleBackground(icon);
                iconElement.style.unityBackgroundImageTintColor = color;

                var margin = (frameSize - size) / 2;
                iconElement.SetElementMargin(margin, margin, margin, margin);

                return iconElement;

            }


        }
    }
}