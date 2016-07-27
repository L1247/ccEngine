using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(MusicObject))]
public class MusicObjectEditor : BaseExcelEditor<MusicObject>
{	
    public override void OnEnable()
    {
        base.OnEnable();
        
        MusicObject data = target as MusicObject;
        
        databaseFields = ExposeProperties.GetProperties(data);
        
        foreach(MusicObjectData e in data.dataArray)
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
            
            MusicObject data = target as MusicObject;
            foreach(MusicObjectData e in data.dataArray)
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
        MusicObject targetData = target as MusicObject;

        string path = targetData.SheetName;
        if (!File.Exists(path))
            return false;

        string sheet = targetData.WorksheetName;

        ExcelQuery query = new ExcelQuery(path, sheet);
        if (query != null && query.IsValid())
        {
            targetData.dataArray = query.Deserialize<MusicObjectData>().ToArray();
			
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            return true;
        }
        else
            return false;
    }
}
