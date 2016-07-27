using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawn : MonoBehaviour
{
    public Sprite[] spr;
    public GameObject Star;
    public float x_Offset = 2;
    public int iPreSpawnAmount = 10;
    List<GameObject> GameObjectPool = new List<GameObject>();
    float callTime;

    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 60;
        //print( ( decimal ) 0 );
        //float f =1f;
        //decimal d ;
        //d = ( decimal ) f;
        //for ( int i = 0 ; i < 10 ; i++ )
        //{
        //    d -= 0.1m;
        //    print( d );
        //} 
        //float f = 1f;
        //double d;
        //d = ( double ) f;
        //for ( int i = 0 ; i < 10 ; i++ )
        //{
        //    d -= 0.1d;
        //    print( d );
        //}
        //for ( int i = 0 ; i < 10 ; i++ )
        //{
        //    f -= 0.1f;
        //    print( f  );
        //    print( Mathf.Abs( f ) < 0.01f );
        //}
        //print( "Next" );
        //f = 0.5f;
        //for ( int i = 0 ; i < 5 ; i++ )
        //{
        //    f -= 0.1f;
        //    print( f );
        //    print( Mathf.Abs( f ) < 0.01f );
        //}
        PreSpawan();
        //StartSpawn();
    }

    public void StartSpawn ()
    {
        Application.targetFrameRate = 60;
        //ccEngine.ccTimeEvent.Instance.CloseValue = 0.05f;
        //ccEngine.ccTimeEvent.Instance.CheckTime = 0.05f;
        //ccEngine.ccTimeEvent.Instance.CloseValue = 0.002f;
        callTime = Time.timeSinceLevelLoad;
        //print( callTime );
        SpawnListData _SpwanListData = null;
        for ( int i = 0 ; i < DataPool.SpawnListDT.SpawnListDataList.Count ; i++ )
        {
            _SpwanListData = DataPool.SpawnListDT.SpawnListDataList[ i ];
            ccEngine.ccTimeEvent.Instance.f_RegEvent( _SpwanListData.Spawntime , SpawnRegCallBack , false , true , _SpwanListData );
        }
        _SpwanListData = DataPool.SpawnListDT.SpawnListDataList
            [ DataPool.SpawnListDT.SpawnListDataList.Count - 1 ];

        //註冊延遲兩秒遊戲結束
        //ccEngine.ccTimeEvent.Instance.f_RegEvent( _SpwanListData.Spawntime + 2 ,
        //        false , OnGameEnd , true );
        //print( ccEngine.ccTimeEvent.Instance.CloseValue );
    }

    private void OnGameEnd ( object data )
    {
        GameMain.GetInstance().OnGameEnd();
        print( "GameEnd" );
        index = 0;
    }

    int index;
    private void SpawnRegCallBack ( object data )
    {
        print( "CallTime : " + ( /*DataPool.SpawnListDT.SpawnListDataList[ index ].Spawntime
            -*/ ( Time.timeSinceLevelLoad - callTime ) ) );
        index++;
        SpawnListData _SpwanListData = ( SpawnListData ) data;
        GameObject Obj = GameObjectPool.Find( obj => obj.activeSelf == false );
        Vector3 v3 = Vector3.right * ( x_Offset * ( -2 + _SpwanListData.Track ) ) + Vector3.up * 7;
        if ( Obj != null )
        {
            Obj.transform.position = v3;
            Obj.SetActive( true );
        }
        else
        {
            Spawn();
        }
        Obj.GetComponent<GameObjectMove>().m_MusicData = new MusicData( _SpwanListData );
        Obj.GetComponent<SpriteRenderer>().sprite = spr[ _SpwanListData.Type - 1 ];
    }
    void Spawn ()
    {
        Vector3 v3 = Vector3.up * 7;
        GameObject Obj = Instantiate( Star , v3 , Quaternion.identity ) as GameObject;
        Obj.SetActive( false );
        GameObjectPool.Add( Obj );
    }
    void PreSpawan ()
    {
        for ( int i = 0 ; i < iPreSpawnAmount ; i++ )
        {
            Spawn();
        }
    }
}
