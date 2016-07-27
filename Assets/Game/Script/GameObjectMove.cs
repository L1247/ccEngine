using UnityEngine;

public class GameObjectMove : MonoBehaviour
{
    Transform tran;
    float Speed = 6;
    float Dead_Y = -4.5f;
    float min_ScorY = -3f;
    float max_ScorY = -2f;
    public MusicData m_MusicData;
    // Use this for initialization
    void Start ()
    {
        tran = transform;
    }
    // Update is called once per frame
    void Update ()
    {
        if ( gameObject.activeSelf )
        {
            Vector3 Postion = tran.position;
            if ( Postion.y <= Dead_Y )
            {
                OnInvisible();
            }
            else
                transform.position -= Vector3.up * Time.deltaTime * Speed;

            //介於Max與min之間
            if ( Postion.y <= max_ScorY && Postion.y > min_ScorY )
            {
                //判斷軌道 來計算分數
                switch ( m_MusicData.iTrackIndex )
                {
                    case 1:
                        if ( Input.GetKeyDown( KeyCode.A ) )
                            OnScore();
                        break;
                    case 2:
                        if ( Input.GetKeyDown( KeyCode.S ) )
                            OnScore();
                        break;
                    case 3:
                        if ( Input.GetKeyDown( KeyCode.D ) )
                            OnScore();
                        break;
                    default:
                        Debug.LogError( "Track 不正確" );
                        break;
                }
            }
        }
    }
    void OnScore ()
    {
        OnInvisible();
        GameMain.GetInstance().OnScoreAdd( m_MusicData.iGainscore );
    }
    void OnInvisible ()
    {
        gameObject.SetActive( false );
    }
}
