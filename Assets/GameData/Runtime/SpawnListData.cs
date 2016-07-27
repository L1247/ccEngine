using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class SpawnListData
{
	[SerializeField]
	float spawntime;
	
	[ExposeProperty]
	public float Spawntime { get {return spawntime; } set { spawntime = value;} }
	
	[SerializeField]
	int type;
	
	[ExposeProperty]
	public int Type { get {return type; } set { type = value;} }
	
	[SerializeField]
	int track;
	
	[ExposeProperty]
	public int Track { get {return track; } set { track = value;} }
	
}