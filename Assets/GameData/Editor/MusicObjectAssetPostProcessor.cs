using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///
/// !!! Machine generated code !!!
///
public class MusicObjectAssetPostprocessor : AssetPostprocessor 
{
    private static readonly string filePath = "Assets/GameData/Excel/MusicGameData.xlsx";
    private static readonly string assetFilePath = "Assets/GameData/Excel/MusicObject.asset";
    private static readonly string sheetName = "MusicObject";
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) 
        {
            if (!filePath.Equals (asset))
                continue;
                
            MusicObject data = (MusicObject)AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(MusicObject));
            if (data == null) {
                data = ScriptableObject.CreateInstance<MusicObject> ();
                data.sheetName = filePath;
                data.worksheetName = sheetName;
                AssetDatabase.CreateAsset ((ScriptableObject)data, assetFilePath);
                //data.hideFlags = HideFlags.NotEditable;
            }
            
            //data.dataArray = new ExcelQuery(filePath, sheetName).Deserialize<MusicObjectData>().ToArray();		

            //ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
            //EditorUtility.SetDirty (obj);

            ExcelQuery query = new ExcelQuery(filePath, sheetName);
            if (query != null && query.IsValid())
            {
                data.dataArray = query.Deserialize<MusicObjectData>().ToArray();

                ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
                EditorUtility.SetDirty (obj);
            }
        }
    }
}
