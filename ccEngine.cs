using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ccEngine
{
    /// <summary>
    /// 帶有一個參數的CallBack
    /// </summary>
    /// <param name="data">object類型 可接受任何類型</param>
    public delegate void ccCallback ( object data );

    /// <summary>
    /// 基礎時間事件類別
    /// </summary>
    public class sTimeEvent
    {
        // Fields
        /// <summary>
        /// 是否一直重複呼叫 ccTimeEvent 適用
        /// </summary>
        public bool m_bRepeat;
        /// <summary>
        /// 此事件是否可被暫停
        /// </summary>
        public bool  m_bUsePause;
        /// <summary>
        /// 內部初始化使用 外部改了沒有特別效果
        /// </summary>
        public bool m_bInit ;
        /// <summary>
        /// 是否回傳剩餘時間 如果值為true則會取代自帶參數object
        /// </summary>
        public bool m_bCallBackSurplusTime;
        /// <summary>
        /// 每個Update中會被呼叫的函數
        /// </summary>
        public ccCallback m_ccCallback ;
        /// <summary>
        /// 最後一次完成後會呼叫的函數
        /// </summary>
        public ccCallback m_ccCallbackComplete ;
        /// <summary>
        /// 自帶參數
        /// </summary>
        public object m_oData ;
        /// <summary>
        /// 一開始的延遲時間
        /// </summary>
        public float m_fDelayTime ;
        /// <summary>
        /// 總共的執行時間，也可理解為剩餘時間
        /// </summary>
        public float  m_fSurplusTime ;
        /// <summary>
        /// 內部使用id 外部修改沒有特別效果
        /// </summary>
        public long iId;

        private int excuteCount;

        /// <summary>
        /// 總執行次數
        /// </summary>
        public int iExcuteCount
        {
            get
            {
                return excuteCount;
            }
            set
            {
                excuteCount = value;
            }
        }

        #region HashTable Fields
        /// <summary>
        /// 延遲時間 float類型
        /// </summary>
        public const string strDelay = "Delay";
        /// <summary>
        /// 總執行時間 float類型
        /// </summary>
        public const string strRuntime = "Runtime";
        /// <summary>
        /// 可帶的參數 object類型
        /// </summary>
        public const string strArgument = "Argument";
        /// <summary>
        /// 每個Update中會被呼叫的函數 ccCallback 類型
        /// </summary>
        public const string strCcCallback = "ccCallback";
        /// <summary>
        /// 最後一次完成後會呼叫的函數 ccCallback 類型
        /// </summary>
        public const string strCcCallbackComplete = "ccCallbackComplete";
        /// <summary>
        /// 此事件是否可以被暫停 bool 類型
        /// </summary>
        public const string strUsePause = "UsePause";
        /// <summary>
        /// 是否回傳剩餘時間 bool類型 如果為true則會取代自帶參數object
        /// </summary>
        public const string strCallBackSurplusTime = "CallBackSurplusTime";
        #endregion

        /// <summary>
        /// 用來註冊有時限的Update事件，例如 畫面中 UI文字 5秒內不斷倒數的功能；
        /// 用來註冊時間內不斷作判斷的事件，例如 遊戲開始後30秒內不斷確定分數有沒有達到一百分
        /// Ex: 5秒內是否有鍵盤按下等
        /// 預設為可以被暫停
        /// </summary>
        /// <param name="fRunTime">總執行時間</param>
        /// <param name="tCCcallback">每次的fun與結束fun都會註冊相同的fun</param>
        public sTimeEvent ( float fRunTime , ccCallback tCCcallback )
        {
            m_fSurplusTime = fRunTime;
            m_ccCallback = tCCcallback;
            m_ccCallbackComplete = tCCcallback;
            m_bUsePause = true;
            m_bCallBackSurplusTime = true;
        }

        /// <summary>
        /// 基礎的sTimeEvent 可自由註冊所有參數
        /// </summary>
        public sTimeEvent ()
        {
        }

        /// <summary>
        /// 轉換Hashtable中的 int to float 與Double to float
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Hashtable CleanArgs ( Hashtable args )
        {
            Hashtable argsCopy = new Hashtable( args.Count );

            foreach ( DictionaryEntry item in args )
            {
                argsCopy.Add( item.Key , item.Value );
            }

            foreach ( DictionaryEntry item in argsCopy )
            {
                if ( item.Value.GetType() == typeof( System.Int32 ) )
                {
                    int original = ( int ) item.Value;
                    float casted = ( float ) original;
                    args[ item.Key ] = casted;
                }
                if ( item.Value.GetType() == typeof( System.Double ) )
                {
                    double original = ( double ) item.Value;
                    float casted = ( float ) original;
                    args[ item.Key ] = casted;
                }
            }
            return args;
        }
    }

    /// <summary>
    /// 註冊一個時間到就會被執行的事件，或者每隔N秒執行,
    /// 例如 五秒後畫面淡出，或者每隔五秒產生一隻怪物等
    /// </summary>
    public class ccTimeEvent : MonoBehaviour
    {
        private float CloseValue = 0.00f;
        private int iKeyId                             = 0;
        private float _fDelayTime                      = 0.1f;
        private Dictionary<int , sTimeEvent> _aDir = new Dictionary<int, sTimeEvent> ();
        private List<int> _aWaitDelEvent           = new List<int>();
        private List<int> _aList                   = new List<int>();
        private static ccTimeEvent _Instance;
        private sTimeEvent _sTimeEvent;
        private bool paused;
        private bool _bInit;

        /// <summary>
        /// 取得ccTimeEvent 的實例，如果沒有則會自己建立
        /// </summary>
        /// <returns></returns>
        public static ccTimeEvent GetInstance ()
        {
            if ( _Instance == null )
            {
                ccUpdateEvent _ccTimeEvent = FindObjectOfType<ccUpdateEvent>();
                if ( _ccTimeEvent != null )
                {
                    _Instance = _ccTimeEvent.gameObject.AddComponent<ccTimeEvent>();
                }
                else
                {
                    GameObject oEngin = new GameObject( "ccEngine" , typeof( ccTimeEvent ) );
                    _Instance = oEngin.GetComponent<ccTimeEvent>();

                    DontDestroyOnLoad( oEngin );
                }

                if ( _Instance == null )
                {
                    Debug.LogError( "init ccTimeEvent Fail" );
                }
            }
            return _Instance;
        }

        /// <summary>
        /// 對 ccTimeEvent 註冊 Invoke 計時器事件 ,
        /// 將會返回一個為int類型的 iEventId 解除事件(f_UnRegEvent)使用，如需使用請自行作變數紀錄。
        /// </summary>
        /// <param name="fDelayTime">一開始Delay的時間</param>
        /// <param name="bRePeat">是否重複執行，預設為不重複，只執行一次</param>
        /// <param name="tccCallback">會被呼叫的函數</param>
        /// <param name="bUsePause">此事件是否可以被暫停，可不填，預設為不可被暫停</param>
        /// <param name="oData">可提供一個可以回傳的參數，如int,GameObject等，可不填，預設為Null</param>
        /// <returns></returns>
        public int f_RegEvent ( float fDelayTime , ccCallback tccCallback , bool bRePeat = false , bool bUsePause = false , object oData = null )
        {
            return this.f_RegEventForTeam( fDelayTime , bRePeat , oData , tccCallback , bUsePause );
        }

        private int f_RegEventForTeam ( float fDelayTime , bool bRePeat , object oData , ccCallback tccCallback , bool bUsePause )
        {
            if ( ( tccCallback != null ) && ( fDelayTime >= 0f ) )
            {
                float fTemp = fDelayTime / _fDelayTime;
                sTimeEvent tClass1 = new sTimeEvent
                {
                    iId = ++iKeyId ,
                    m_fDelayTime = fDelayTime ,
                    m_fSurplusTime = fDelayTime ,
                    //4捨5入  3.05f = 31次, 3.49f = 30次  4捨5入取到小數點第二位
                    iExcuteCount = CommonClass.Round( fTemp , 0 ) ,
                    m_bRepeat = bRePeat ,
                    m_oData = oData ,
                    m_ccCallback = tccCallback ,
                    m_bUsePause = bUsePause
                };
                //初始化已經在跑了要補加一次
                if ( _bInit )
                    tClass1.iExcuteCount++;
                _aDir.Add( iKeyId , tClass1 );
                _aList.Add( iKeyId );
                this.Init();
                return ( int ) tClass1.iId;
            }
            return -99;
        }
        /// <summary>
        /// 回復所有(可被暫停的)事件
        /// </summary>
        public void f_Resume ()
        {
            this.paused = false;
        }

        /// <summary>
        /// 暫停當初註冊時設定可被暫停的事件
        /// </summary>
        public void f_Pause ()
        {
            this.paused = true;
        }

        /// <summary>
        /// 解除特定iEventId的事件
        /// </summary>
        /// <param name="iEventId">填入系統返回的ID</param>
        public void f_UnRegEvent ( int iEventId )
        {
            if ( _aList.Exists( iId => iId == iEventId ) != false )
            {
                this._aDir.Remove( iEventId );
                this._aList.Remove( iEventId );
                CheckInvokeAmount();
            }
            else
            {
                Debug.Log( "ccTimeEvent 找不到此 iEventId : " + iEventId );
            }
        }

        /// <summary>
        /// 解除所有已註冊的事件
        /// </summary>
        public void f_UnRegAllEvent ()
        {
            this._aDir.Clear();
            this._aList.Clear();
            CheckInvokeAmount();
        }

        /// <summary>
        /// 取得指定ID的事件目前剩餘的執行次數，每個次數0.1秒，可自行換算
        /// 找不到時，回傳int類型 數值0
        /// </summary>
        /// <param name="iEventID"></param>
        /// <returns></returns>
        public int f_GetTimeEventExcuteCount ( int iEventID )
        {
            if ( _aDir.ContainsKey( iEventID ) )
                return _aDir[ iEventID ].iExcuteCount;
            else
            {
                Debug.Log( "f_GetTimeEventExcuteCount 找不到此 iEventID 事件" );
                return 0;
            }
        }

        /// <summary>
        /// 增加指定ID的事件目前剩餘的執行次數，每個次數為0.1秒，可自行換算
        /// </summary>
        /// <param name="iEventID">該時間事件的ID</param>
        /// <param name="fSecond">增加的秒數(float)，可為負的(會相減)，如果相減後低於0則直接完成計時</param>
        /// <returns></returns>
        public void f_AddTimeEventExcuteCount ( int iEventID , float fSecond )
        {
            int _ExcCount = Mathf.CeilToInt( fSecond / this._fDelayTime );

            if ( _aDir.ContainsKey( iEventID ) )
                _aDir[ iEventID ].iExcuteCount += _ExcCount;
            else
            {
                Debug.Log( "f_AddTimeEventExcuteCount 找不到此 iEventID 事件" );
            }
        }

        private void Init ()
        {
            if ( !this._bInit )
            {
                this._bInit = true;
                base.InvokeRepeating( "Xsfdsfasdfadsfa" , ( float ) this._fDelayTime , ( float ) this._fDelayTime );
                this.f_Resume();
            }
        }

        private void Xsfdsfasdfadsfa ()
        {
            this.CustomUpdate();
        }
        private void CustomUpdate ()
        {
            if ( this._aList.Count > 0 )
            {
                for ( int i = 0 ; i < this._aList.Count ; i++ )
                {
                    this._sTimeEvent = ( sTimeEvent ) _aDir[ this._aList[ i ] ];
                    if ( !this._sTimeEvent.m_bUsePause || !this.paused )
                    {
                        this._sTimeEvent.iExcuteCount -= 1;
                        if (/* this._sTimeEvent.m_fSurplusTime <=  CloseValue */this._sTimeEvent.iExcuteCount <= 0 )
                        {
                            if ( this._sTimeEvent.m_ccCallback == null )
                            {
                                this._aWaitDelEvent.Add( ( int ) this._sTimeEvent.iId );
                            }
                            else
                            {
                                this._sTimeEvent.m_ccCallback( this._sTimeEvent.m_oData );
                                if ( this._sTimeEvent.m_bRepeat )
                                {
                                    float fTemp = this._sTimeEvent.m_fDelayTime / this._fDelayTime;
                                    this._sTimeEvent.iExcuteCount = CommonClass.Round( fTemp , 0 );
                                }
                                else
                                {
                                    this._aWaitDelEvent.Add( ( int ) this._sTimeEvent.iId );
                                }
                            }
                        }
                    }
                }
                if ( this._aWaitDelEvent.Count > 0 )
                {
                    foreach ( int num2 in this._aWaitDelEvent )
                    {
                        this._aDir.Remove( num2 );
                        this._aList.Remove( num2 );
                    }
                    this._aWaitDelEvent.Clear();
                    CheckInvokeAmount();
                }
            }
        }

        private void CheckInvokeAmount ()
        {
            if ( _aList.Count == 0 )
            {
                CancelInvoke( "Xsfdsfasdfadsfa" );
                _bInit = false;
            }
        }

    }

    /// <summary>
    /// 用來註冊有時限的Update事件，例如 畫面中 UI文字 5秒內不斷倒數的功能,
    /// 或者 時間內不斷作判斷的事件，例如 遊戲開始後30秒內不斷確定分數有沒有達到一百分
    /// </summary>
    public class ccUpdateEvent : MonoBehaviour
    {
        private int iKeyId;
        private Dictionary<int , sTimeEvent> _aDir = new Dictionary<int, sTimeEvent> ();
        private List<int> _aWaitDelEvent           = new List<int>();
        private List<int> _aList                   = new List<int>();
        private sTimeEvent _sTimeEvent;
        private bool paused;
        private bool _bInit;
        private static ccUpdateEvent _Instance;

        /// <summary>
        /// 可使用此sTimeEvent當作參數  直接修改就可以不用儲存在自己的Class中了
        /// </summary>
        public static sTimeEvent m_sTimeEvent = new sTimeEvent ();

        /// <summary>
        /// 取得ccUpdateEvent 的實例，如果沒有則會自己建立
        /// </summary>
        /// <returns></returns>
        public static ccUpdateEvent GetInstance ()
        {
            if ( _Instance == null )
            {
                ccTimeEvent _ccTimeEvent = FindObjectOfType<ccTimeEvent>();
                if ( _ccTimeEvent != null )
                {
                    _Instance = _ccTimeEvent.gameObject.AddComponent<ccUpdateEvent>();
                }
                else
                {
                    GameObject oEngin = new GameObject( "ccEngine" , typeof( ccUpdateEvent ) );
                    _Instance = oEngin.GetComponent<ccUpdateEvent>();

                    DontDestroyOnLoad( oEngin );
                }
                //_Instance = FindObjectOfType( typeof( ccUpdateEvent ) ) as ccUpdateEvent;
                if ( _Instance == null )
                {
                    Debug.LogError( "init ccUpdateEvent Fail" );
                }
            }
            return _Instance;
        }
        /// <summary>
        /// 解除特定iEventId的事件
        /// </summary>
        /// <param name="iEventId">填入系統返回的ID</param>
        public void f_UnRegEvent ( int iEventId )
        {
            if ( _aList.Exists( iId => iId == iEventId ) != false )
            {
                this._aDir.Remove( iEventId );
                this._aList.Remove( iEventId );
            }
            else
            {
                Debug.Log( "ccUptateEvent 找不到此 iEventId : " + iEventId );
            }
        }
        /// <summary>
        /// 解除所有已註冊的事件
        /// </summary>
        public void f_UnRegAllEvent ()
        {
            this._aDir.Clear();
            this._aList.Clear();
        }

        /// <summary>
        /// 暫停當初註冊時設定可被暫停的事件
        /// </summary>
        public void f_Pause ()
        {
            this.paused = true;
        }

        /// <summary>
        /// 回復所有(可被暫停的)事件
        /// </summary>
        public void f_Resume ()
        {
            this.paused = false;
        }


        /// <summary>
        /// 對 ccUpdateEvent 註冊 Update 事件 ,
        /// 將會返回一個為int類型的 iEventId 解除事件(f_UnRegEvent)使用，如需使用請自行作變數紀錄。
        /// </summary>
        /// <param name="fDelayTime">一開始Delay的時間，預設為0秒</param>
        /// <param name="fRunTime">總共執行中的時間</param>
        /// <param name="tccCallback">每幀會被呼叫的函數</param>
        /// <param name="tccCallbackComplete">結束後會被呼叫的函數</param>
        /// <param name="oData">可提供一個可以回傳的參數，如int,GameObject等，可不填，預設為Null</param>
        /// <param name="bUsePause">此事件是否可以被暫停，可不填，預設為不可被暫停</param>
        /// <param name="bCallBackSurplusTime">是否傳回剩餘的時間，預設為不回傳，只回傳參數</param>
        /// <returns></returns>
        public int f_RegEvent ( float fRunTime , ccCallback tccCallback , float fDelayTime = 0 ,
            ccCallback tccCallbackComplete = null , object oData = null , bool bUsePause = false ,
            bool bCallBackSurplusTime = false )
        {
            return this.f_RegEventForTeam( fDelayTime , fRunTime , oData , tccCallback ,
                tccCallbackComplete , bUsePause , bCallBackSurplusTime );
        }

        /// <summary>
        /// 至少要給予m_fSurplusTime、m_ccCallback 否則將不會執行
        /// </summary>
        /// <param name="tsTimeEvent">給予一個sTimeEvent類別的物件
        /// 可使用ccUpdateEvent中的m_sTimeEvent去new出來</param>
        /// <returns></returns>
        public int f_RegEvent ( sTimeEvent tsTimeEvent )
        {
            return this.f_RegEventForTeam( ( float ) tsTimeEvent.m_fDelayTime ,
                ( float ) tsTimeEvent.m_fSurplusTime , tsTimeEvent.m_oData ,
                tsTimeEvent.m_ccCallback , tsTimeEvent.m_ccCallbackComplete ,
                tsTimeEvent.m_bUsePause , tsTimeEvent.m_bCallBackSurplusTime );
        }

        /// <summary>
        /// 使用Hashtable
        /// </summary>
        /// <param name="hashTable"></param>
        /// <returns></returns>
        public int f_RegEvent ( System.Collections.Hashtable hashTable )
        {
            hashTable = sTimeEvent.CleanArgs( hashTable );
            if ( hashTable[ sTimeEvent.strDelay ] != null &&
                hashTable[ sTimeEvent.strDelay ] is float )
                m_sTimeEvent.m_fDelayTime = ( float ) hashTable[ sTimeEvent.strDelay ];

            if ( hashTable[ sTimeEvent.strRuntime ] != null &&
                hashTable[ sTimeEvent.strRuntime ] is float )
                m_sTimeEvent.m_fSurplusTime = ( float ) hashTable[ sTimeEvent.strRuntime ];

            if ( hashTable[ sTimeEvent.strArgument ] != null &&
                hashTable[ sTimeEvent.strArgument ] is object )
                m_sTimeEvent.m_oData = hashTable[ sTimeEvent.strArgument ];

            if ( hashTable[ sTimeEvent.strCcCallback ] != null &&
                hashTable[ sTimeEvent.strCcCallback ] is ccCallback )
                m_sTimeEvent.m_ccCallback = ( ccCallback ) hashTable[ sTimeEvent.strCcCallback ];

            if ( hashTable[ sTimeEvent.strCcCallbackComplete ] != null &&
                hashTable[ sTimeEvent.strCcCallbackComplete ] is ccCallback )
                m_sTimeEvent.m_ccCallbackComplete =
                    ( ccCallback ) hashTable[ sTimeEvent.strCcCallbackComplete ];

            if ( hashTable[ sTimeEvent.strUsePause ] != null &&
                hashTable[ sTimeEvent.strUsePause ] is bool )
                m_sTimeEvent.m_bUsePause = ( bool ) hashTable[ sTimeEvent.strUsePause ];

            if ( hashTable[ sTimeEvent.strCallBackSurplusTime ] != null &&
                hashTable[ sTimeEvent.strCallBackSurplusTime ] is bool )
                m_sTimeEvent.m_bCallBackSurplusTime =
                    ( bool ) hashTable[ sTimeEvent.strCallBackSurplusTime ];

            return this.f_RegEvent( m_sTimeEvent );
        }

        private int f_RegEventForTeam ( float fDelayTime , float fRunTime , object oData ,
            ccCallback tccCallback , ccCallback tccCallbackComplete , bool bUsePause ,
            bool bCallBackSurplusTime )
        {
            if ( ( tccCallback != null ) && ( fDelayTime >= 0f ) )
            {
                sTimeEvent tClass1 = new sTimeEvent
                {
                    iId = ++iKeyId ,
                    m_fDelayTime = fDelayTime ,
                    m_fSurplusTime = fRunTime ,
                    m_oData = oData ,
                    m_ccCallback = tccCallback ,
                    m_ccCallbackComplete = tccCallbackComplete ,
                    m_bUsePause = bUsePause ,
                    m_bCallBackSurplusTime = bCallBackSurplusTime
                };
                _aDir.Add( iKeyId , tClass1 );
                _aList.Add( iKeyId );

                if ( this.enabled == false )
                    this.enabled = true;
                return ( int ) tClass1.iId;
            }
            return -99;
        }
        private void Update ()
        {
            if ( this._aList.Count > 0 )
            {
                for ( int i = 0 ; i < this._aList.Count ; i++ )
                {
                    this._sTimeEvent = _aDir[ this._aList[ i ] ];
                    if ( !this._sTimeEvent.m_bUsePause || !this.paused )
                    {
                        if ( this._sTimeEvent.m_bInit == false )
                        {
                            this._sTimeEvent.m_bInit = true;
                            return;
                        }
                        if ( this._sTimeEvent.m_fDelayTime > 0 )
                        {
                            this._sTimeEvent.m_fDelayTime -= Time.deltaTime;
                        }
                        else
                        {
                            this._sTimeEvent.m_fSurplusTime -= Time.deltaTime;
                            if ( this._sTimeEvent.m_fSurplusTime > 0f )
                            {
                                if ( this._sTimeEvent.m_ccCallback == null )
                                {
                                    this._aWaitDelEvent.Add( ( int ) this._sTimeEvent.iId );
                                }
                                else
                                {
                                    if ( this._sTimeEvent.m_bCallBackSurplusTime == false )
                                        this._sTimeEvent.m_ccCallback( this._sTimeEvent.m_oData );
                                    else
                                        this._sTimeEvent.m_ccCallback( this._sTimeEvent.m_fSurplusTime );
                                }
                            }
                            else
                            {
                                this._aWaitDelEvent.Add( ( int ) this._sTimeEvent.iId );
                                if ( this._sTimeEvent.m_ccCallbackComplete != null )
                                {
                                    if ( this._sTimeEvent.m_bCallBackSurplusTime == false )
                                        this._sTimeEvent.m_ccCallbackComplete( this._sTimeEvent.m_oData );
                                    else
                                        this._sTimeEvent.m_ccCallbackComplete
                                            ( 0f );
                                }
                            }

                        }
                    }
                }
                if ( this._aWaitDelEvent.Count > 0 )
                {
                    foreach ( int num2 in this._aWaitDelEvent )
                    {
                        this._aDir.Remove( num2 );
                        this._aList.Remove( num2 );
                    }
                    this._aWaitDelEvent.Clear();
                }
            }
            else
            {
                this.enabled = false;
            }
        }
    }

    /// <summary>
    /// 可註冊一個搖晃物體的事件，結束時也可註冊函式等著被呼叫
    /// 註冊的物體不限為攝影機，普通遊戲物件都可
    /// </summary>
    public class CameraShake
    {
        /// <summary>
        /// 四種搖晃類型，
        /// 1.垂直
        /// 2.水平
        /// 3.垂直 + 水平
        /// 4.三軸
        /// </summary>
        public enum eShakeType
        {
            /// <summary>
            /// 1.垂直
            /// </summary>
            Vertical,
            /// <summary>
            /// 2.水平
            /// </summary>
            Horizontal,
            /// <summary>
            /// 3.垂直 + 水平
            /// </summary>
            VerticalAndHorizontal,
            /// <summary>
            /// 4.三軸
            /// </summary>
            Sphere,
        }

        /// <summary>
        /// 搖晃物件；
        /// fShakeAmount為 一倍時；位移量各為1，請視情況修改fShakeAmount(力道)
        /// </summary>
        /// <param name="gameobject">要使用的攝影機或遊戲物件</param>
        /// <param name="fShakeDuration">總執行時間，預設為0秒</param>
        /// <param name="fDelayTime">延遲呼叫的時間，預設為0秒</param>
        /// <param name="fShakeAmount">搖晃的力道,最後會乘上這個數值,可以當作倍率,預設為一倍</param>
        /// <param name="_eShakeType">搖晃的類型；目前提供 垂直、水平、垂直+水平、圓球(三軸)
        /// 預設為垂直+水平</param>
        /// <param name="tccCalllbackComplete">完成事件後將會被呼叫的函式，會回傳原本註冊
        /// 的GameObject</param>
        /// <param name="bUsePause">是否可被暫停，預設為不可被暫停</param>
        public static int ShakeGameObject ( GameObject gameobject , float fShakeDuration = 0f ,
            float fDelayTime = 0 , float fShakeAmount = 1f ,
            eShakeType _eShakeType = eShakeType.VerticalAndHorizontal ,
            ccCallback tccCalllbackComplete = null , bool bUsePause = false )
        {
            object[] obj = new object[ 5 ];
            obj[ 0 ] = gameobject.transform;
            obj[ 1 ] = gameobject.transform.localPosition;
            obj[ 2 ] = fShakeAmount;
            obj[ 3 ] = _eShakeType;
            obj[ 4 ] = tccCalllbackComplete;
            return ccUpdateEvent.GetInstance().f_RegEvent( fShakeDuration , Shake , fDelayTime , ShakeComplete , obj , bUsePause );
        }

        private static void Shake ( object data )
        {
            Vector3 v = Vector3.zero;

            object[] obj = ( object[] ) data;
            Transform camTransform = ( Transform ) obj[ 0 ];
            Vector3 originalPos = ( Vector3 ) obj[ 1 ];
            float fShakeAmount = ( float ) obj[ 2 ];
            eShakeType m_ShakeType = ( eShakeType ) obj[ 3 ];
            switch ( m_ShakeType )
            {
                case eShakeType.Vertical:
                    v = Vector3.up * UnityEngine.Random.Range( -1.0f , 1.0f );
                    break;
                case eShakeType.Horizontal:
                    v = Vector3.right * UnityEngine.Random.Range( -1.0f , 1.0f );
                    break;
                case eShakeType.VerticalAndHorizontal:
                    v = Vector3.right * UnityEngine.Random.Range( -1.0f , 1.0f ) +
                        Vector3.up * UnityEngine.Random.Range( -1.0f , 1.0f );
                    break;
                case eShakeType.Sphere:
                    v = UnityEngine.Random.insideUnitSphere;
                    break;
                default:
                    Debug.LogError( "沒有選擇任何模式，當初註冊的eShakeType不存在於預設Enum中" );
                    break;
            }
            if ( camTransform != null )
                camTransform.localPosition = originalPos + v * fShakeAmount;

        }
        private static void ShakeComplete ( object data )
        {
            object[] obj = ( object[] ) data;
            Transform camTransform = ( Transform ) obj[ 0 ];
            Vector3 originalPos = ( Vector3 ) obj[ 1 ];
            ccCallback _ccCallback = ( ccCallback ) obj[ 4 ];
            if ( camTransform )
            {
                _ccCallback( camTransform.gameObject );
                camTransform.localPosition = originalPos;
            }
            else
            {
                Debug.LogError( "CameraShake 找不到當初註冊搖晃的物件" );
                _ccCallback( null );
            }

        }
    }

    /// <summary>
    /// 通用類別
    /// </summary>
    public class CommonClass
    {
        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="value">要轉換的數值</param>
        /// <param name="digit">取到小數點後第幾位數，0：第一位 1:第二位
        /// Ex：Round(30.5) = 31, Round(30.4) = 30</param>
        /// <returns></returns>
        public static int Round ( double value , int digit )
        {
            double vt = System.Math.Pow( 10 , digit );
            //1.乘以倍数 + 0.5
            double vx = value * vt + 0.5;
            //2.向下取整
            double temp = System.Math.Floor( vx );
            //3.再除以倍数
            return ( int ) ( temp / vt );
        }
    }
}
