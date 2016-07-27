public class DataPool  {
    public static MusicObject MusicObjectDT;
    public static SpawnList SpawnListDT;

    public static void f_InitPool ()
    {
        MusicObjectDT = UnityEngine.Resources.Load<MusicObject>( "ExcelData/MusicObject" );
        SpawnListDT = UnityEngine.Resources.Load<SpawnList>( "ExcelData/SpawnList" );
    }
}
