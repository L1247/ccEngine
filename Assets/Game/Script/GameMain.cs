using ccEngine;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public Sprite[] NumSprite;
    public Image UI_TimeImg;
    public Button UI_StartGame;
    public GameObject GameOverUI ;
    public GameObject ScorePoint ;
    private Image Img_Ten , Img_Num;
    private GameObjectSpawn m_GameObjectSpawn;
    private static GameMain _Instance = null;
    private int GameScore;
    public static GameMain GetInstance ()
    {
        if ( !_Instance )
        {
            _Instance = ( GameMain ) FindObjectOfType( typeof( GameMain ) );
            if ( !_Instance )
            {
                Debug.LogError( "init GameMain Fail" );
            }
        }
        return _Instance;
    }

    public void OnScoreAdd ( int score )
    {
        GameScore += score;
        print( GameScore );
    }
    float f = 0;
    object o;
    // Use this for initialization
    void Start ()
    {
        DataPool.f_InitPool();
        m_GameObjectSpawn = GetComponent<GameObjectSpawn>();
        Img_Ten = GameOverUI.transform.FindChild( "Ten" ).GetComponent<Image>();
        Img_Num = GameOverUI.transform.FindChild( "Num" ).GetComponent<Image>();

        UI_StartGame.onClick.AddListener( delegate ()
        {
            this.OnStartGameClick();
        } );

        UI_TimeImg.gameObject.SetActive( false );
        GameOverUI.SetActive( false );
    }
    private void OnStartGameClick ()
    {
        UI_StartGame.gameObject.SetActive( false );
        InitGame();
    }
    public void OnGameEnd ()
    {
        int iTen = GameScore / 10;
        int iNum = GameScore % 10;
        Img_Ten.sprite = GetUIImage( iTen );
        Img_Num.sprite = GetUIImage( iNum );
        UI_StartGame.gameObject.SetActive( true );
        GameOverUI.SetActive( true );
        ScorePoint.SetActive( false );
        GameScore = 0;
    }
    private void InitGame ()
    {
        ScorePoint.SetActive( true );
        GameOverUI.SetActive( false );
        UI_TimeImg.gameObject.SetActive( true );
        UI_TimeImg.sprite = GetUIImage( 3 );
        ccUpdateEvent.m_sTimeEvent = new sTimeEvent( 3 , CountDownCallBack );
        ccUpdateEvent.Instance.f_RegEvent( ccUpdateEvent.m_sTimeEvent );
    }

    private void CountDownCallBack ( object data , float fTime )
    {
        int CeilTime = Mathf.CeilToInt( fTime );
        UI_TimeImg.sprite = GetUIImage( CeilTime );
        if ( CeilTime == 0 )
            ccTimeEvent.Instance.f_RegEvent( 0.2f , DelayDisable );
    }

    private void DelayDisable ( object data )
    {
        UI_TimeImg.gameObject.SetActive( false );
        UI_TimeImg.sprite = null;
        m_GameObjectSpawn.StartSpawn();
    }

    Sprite GetUIImage ( int num )
    {
        return NumSprite[ num ];
    }
}
