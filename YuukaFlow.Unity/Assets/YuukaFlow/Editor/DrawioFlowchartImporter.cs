
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace YuukaFlow.Unity
{
    [ScriptedImporter(1, "drawio")]
    public class DrawioFlowchartImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            try
            {
                var xml = System.IO.File.ReadAllText(ctx.assetPath);
                var asset = ScriptableObject.CreateInstance<DrawioFlowchartAsset>();
                asset.SetRawData(xml);

                ctx.AddObjectToAsset("flowchart", asset);
                ctx.SetMainObject(asset);

                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GUIDs.FlowchartFileIcon));
                EditorGUIUtility.SetIconForObject(asset, icon);
            }
            catch (System.Exception e)
            {
                ctx.LogImportError($"Failed to import drawio file: {ctx.assetPath}\n{e}");
            }
        }
    }
}