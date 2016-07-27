using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(SpawnList))]
public class SpawnListEditor : BaseExcelEditor<SpawnList>
{	
    public override void OnEnable()
    {
        base.OnEnable();
        
        SpawnList data = target as SpawnList;
        
        databaseFields = ExposeProperties.GetProperties(data);
        
        foreach(SpawnListData e in data.dataArray)
        {
            dataFields = ExposeProperties.GetProperties(e);
            pInfoList.Add(dataFields);
        }
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //DrawDefaultInspector();
        if (GUI.changed)
        {
            pInfoList.Clear();
            
            SpawnList data = target as SpawnList;
            foreach(SpawnListData e in data.dataArray)
            {
                dataFields = ExposeProperties.GetProperties(e);
                pInfoList.Add(dataFields);
            }
            
            EditorUtility.SetDirty(target);
            Repaint();
        }
    }
    
    public override bool Load()
    {
        SpawnList targetData = target as SpawnList;

        string path = targetData.SheetName;
        if (!File.Exists(path))
            return false;

        string sheet = targetData.WorksheetName;

        ExcelQuery query = new ExcelQuery(path, sheet);
        if (query != null && query.IsValid())
        {
            targetData.dataArray = query.Deserialize<SpawnListData>().ToArray();
			
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            return true;
        }
        else
            return false;
    }
}
