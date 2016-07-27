using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class MusicObjectData
{
	[SerializeField]
	int type;
	
	[ExposeProperty]
	public int Type { get {return type; } set { type = value;} }
	
	[SerializeField]
	int speed;
	
	[ExposeProperty]
	public int Speed { get {return speed; } set { speed = value;} }
	
	[SerializeField]
	int gainscore;
	
	[ExposeProperty]
	public int Gainscore { get {return gainscore; } set { gainscore = value;} }
	
	[SerializeField]
	string spritename;
	
	[ExposeProperty]
	public string Spritename { get {return spritename; } set { spritename = value;} }
	
}