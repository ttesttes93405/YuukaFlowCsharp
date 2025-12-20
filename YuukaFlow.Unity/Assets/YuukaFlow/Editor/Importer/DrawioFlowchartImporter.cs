
using UnityEditor.AssetImporters;
using UnityEngine;

namespace YuukaFlow.Unity
{

    [ScriptedImporter(1, "drawio")]
    public class DrawioFlowchartImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // 1. 讀原始檔案
            // 2. 解析資料
            // 3. 建立 ScriptableObject / Texture / Mesh
            // 4. ctx.AddObjectToAsset
            // 5. ctx.SetMainObject

            var xml = System.IO.File.ReadAllText(ctx.assetPath);
            var asset = ScriptableObject.CreateInstance<DrawioFlowchartAsset>();
            asset.SetRawData(xml);

            ctx.AddObjectToAsset("flowchart", asset);
            ctx.SetMainObject(asset);
        }
    }
}