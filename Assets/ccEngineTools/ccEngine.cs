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
    /// 帶有兩個參數的CallBack，通常給UpdateEvent使用
    /// </summary>
    /// <param name="data">註冊的參數回傳</param>
    /// <param name="data2">，第二個為事件剩餘時間</param>
    public delegate void ccCallbackV2 ( object data , float data2 );

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
        /// 每個Update中會被呼叫的函數
        /// </summary>
        public ccCallbackV2 m_ccCallbackV2 ;
        /// <summary>
        /// 最後一次完成後會呼叫的函數
        /// </summary>
        public ccCallbackV2 m_ccCallbackV2Complete ;
        /// <summary>
        /// ccTimeEvent結束呼叫
        /// </summary>
        public ccCallback m_ccCallback ;
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
        #endregion

        /// <summary>
        /// 用來註冊有時限的ccUpdateEvent，例如 畫面中 UI文字 5秒內不斷倒數的功能；
        /// 用來註冊時間內不斷作判斷的事件，例如 遊戲開始後30秒內不斷確定分數有沒有達到一百分
        /// Ex: 5秒內是否有鍵盤按下等
        /// 預設為可以被暫停
        /// </summary>
        /// <param name="fRunTime">總執行時間</param>
        /// <param name="tCCcallback">每次的fun與結束fun都會註冊相同的fun</param>
        public sTimeEvent ( float fRunTime , ccCallbackV2 tCCcallbackV2 )
        {
            m_fSurplusTime = fRunTime;
            m_ccCallbackV2 = tCCcallbackV2;
            m_ccCallbackV2Complete = tCCcallbackV2;
            m_bUsePause = true;
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
        private int iKeyId                                 = 0;
        private float _fDelayTime                          = 0.1f;
        private List<sTimeEvent> _aWaitDelEvent            = new List<sTimeEvent>();
        private List<sTimeEvent> _aTimeEventList           = new List<sTimeEvent>();
        private sTimeEvent _sTimeEvent;
        private bool paused;
        private bool _bInit;

        private static ccTimeEvent _Instance;
        /// <summary>
        /// 取得ccTimeEvent 的實例，如果沒有則會自己建立
        /// </summary>
        public static ccTimeEvent Instance
        {
            get
            {
                if ( _Instance == null )
                {
                    if ( ccCommonClass.ccEngineSingletonObj == null )
                    {
                        GameObject oEngin = new GameObject( "ccEngine" );
                        ccCommonClass.ccEngineSingletonObj = oEngin;
                        DontDestroyOnLoad( oEngin );
                    }
                    _Instance = ccCommonClass.ccEngineSingletonObj.AddComponent<ccTimeEvent>();

                    if ( _Instance == null )
                    {
                        Debug.LogError( "Init ccTimeEvent Fail" );
                    }
                }
                return _Instance;
            }
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
                    //4捨5入  3.05f = 31次, 3.049f = 30次 ，4捨5入取到小數點第二位
                    iExcuteCount = ccCommonClass.Round( fTemp , 0 ) ,
                    m_bRepeat = bRePeat ,
                    m_oData = oData ,
                    m_ccCallback = tccCallback ,
                    m_bUsePause = bUsePause
                };
                _aTimeEventList.Add( tClass1 );
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
            int iIndex = _aTimeEventList.FindIndex( e => e.iId == iEventId );
            if ( iIndex != -1 )
            {
                this._aTimeEventList.RemoveAt( iIndex );
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
            this._aTimeEventList.Clear();
            CheckInvokeAmount();
        }

        /// <summary>
        /// 取得指定ID的事件目前剩餘的執行時間
        /// 找不到時，回傳float類型 數值 -99
        /// </summary>
        /// <param name="iEventID"></param>
        /// <returns></returns>
        public float f_GetTimeEventExcuteTime ( int iEventID )
        {
            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _sTimeEvent != null )
                return _sTimeEvent.iExcuteCount * this._fDelayTime;
            else
            {
                Debug.Log( "ccTimeEvent : f_GetTimeEventExcuteTime 找不到此 " + iEventID
                   + " 事件" );
                return -99;
            }
        }

        /// <summary>
        /// 增加指定ID的事件執行時間
        /// 可為負數，如果相加後小於等於0，則直接完成計時
        /// </summary>
        /// <param name="iEventID">該時間事件的ID</param>
        /// <param name="fSecond">增加的秒數(float)，可為負的(會相減)，如果相減後低於0則直接完成計時，
        /// 4捨5入  3.05f = 31次, 3.049f = 30次 ，4捨5入取到小數點第二位</param>
        /// <returns></returns>
        public void f_AddTimeEventExcuteTime ( int iEventID , float fSecond )
        {
            float fTemp = fSecond / this._fDelayTime;
            int _ExcCount = ccCommonClass.Round( fTemp , 0 );

            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _sTimeEvent != null )
                _sTimeEvent.iExcuteCount += _ExcCount;
            else
            {
                Debug.Log( "ccTimeEvent : f_AddTimeEventExcuteTime 找不到此 " + iEventID
                    + " 事件" );
            }
        }

        /// <summary>
        /// 設定指定ID的事件執行時間
        /// fSecond可為負數，如果fSecond小於等於0，則直接完成計時
        /// </summary>
        /// <param name="iEventID">該時間事件的ID</param>
        /// <param name="fSecond">設定的秒數(float)，可為負的(會相減)，如果相減後低於0則直接完成計時，
        /// 4捨5入  3.05f = 31次, 3.049f = 30次 ，4捨5入取到小數點第二位</param>
        /// <returns></returns>
        public void f_SetTimeEventExcuteTime ( int iEventID , float fSecond )
        {
            float fTemp = fSecond / this._fDelayTime;
            int _ExcCount = ccCommonClass.Round( fTemp , 0 );

            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _sTimeEvent != null )
                _sTimeEvent.iExcuteCount = _ExcCount;
            else
            {
                Debug.Log( "ccTimeEvent : f_AddTimeEventExcuteCount 找不到此 " + iEventID
                   + " 事件" );
            }
        }

        private void Init ()
        {
            //檢查是否初始化過，只創造一個InvokeRepeating
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
            if ( this._aTimeEventList.Count > 0 )
            {
                for ( int i = 0 ; i < this._aTimeEventList.Count ; i++ )
                {
                    this._sTimeEvent = this._aTimeEventList[ i ];
                    if ( !this._sTimeEvent.m_bUsePause || !this.paused )
                    {
                        this._sTimeEvent.iExcuteCount -= 1;
                        if ( this._sTimeEvent.iExcuteCount <= 0 )
                        {
                            if ( this._sTimeEvent.m_ccCallback == null )
                            {
                                this._aWaitDelEvent.Add( this._sTimeEvent );
                            }
                            else
                            {
                                this._sTimeEvent.m_ccCallback( this._sTimeEvent.m_oData );
                                if ( this._sTimeEvent.m_bRepeat )
                                {
                                    float fTemp = this._sTimeEvent.m_fDelayTime / this._fDelayTime;
                                    this._sTimeEvent.iExcuteCount = ccCommonClass.Round( fTemp , 0 );
                                }
                                else
                                {
                                    this._aWaitDelEvent.Add( this._sTimeEvent );
                                }
                            }
                        }
                    }
                }
                if ( this._aWaitDelEvent.Count > 0 )
                {
                    for ( int i = 0 ; i < _aWaitDelEvent.Count ; i++ )
                    {
                        this._aTimeEventList.Remove( _aWaitDelEvent[ i ] );
                    }
                    this._aWaitDelEvent.Clear();
                    CheckInvokeAmount();
                }
            }
        }

        private void CheckInvokeAmount ()
        {
            if ( _aTimeEventList.Count == 0 )
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
        private List<sTimeEvent> _aWaitDelEvent         = new List<sTimeEvent>();
        private List<sTimeEvent> _aTimeEventList        = new List<sTimeEvent>();
        private sTimeEvent _sTimeEvent;
        private bool paused;
        private bool _bInit;

        /// <summary>
        /// 可使用此sTimeEvent當作參數  直接修改就可以不用儲存在自己的Class中了
        /// </summary>
        public static sTimeEvent m_sTimeEvent = new sTimeEvent ();

        private static ccUpdateEvent _Instance;
        /// <summary>
        /// 取得ccUpdateEvent 的實例，如果沒有則會自己建立。
        /// </summary>
        public static ccUpdateEvent Instance
        {
            get
            {
                if ( _Instance == null )
                {
                    if ( ccCommonClass.ccEngineSingletonObj == null )
                    {
                        GameObject oEngin = new GameObject( "ccEngine" );
                        ccCommonClass.ccEngineSingletonObj = oEngin;
                        DontDestroyOnLoad( oEngin );
                    }
                    _Instance = ccCommonClass.ccEngineSingletonObj.AddComponent<ccUpdateEvent>();

                    if ( _Instance == null )
                    {
                        Debug.LogError( "Init ccTimeEvent Fail" );
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        /// 對 ccUpdateEvent 註冊 Update 事件 ,
        /// 將會返回一個為int類型的 iEventId 解除事件(f_UnRegEvent)使用，如需使用請自行作變數紀錄。
        /// </summary>
        /// <param name="fDelayTime">一開始Delay的時間，預設為0秒</param>
        /// <param name="fRunTime">總共執行中的時間</param>
        /// <param name="tccCallback">每幀會被呼叫的函數</param>
        /// <param name="tccCallbackComplete">最後一幀結束後會被呼叫的函數，如果為空，則預設與tccCallback同函數</param>
        /// <param name="oData">可提供一個可以回傳的參數，如int,GameObject等，可不填，預設為Null</param>
        /// <param name="bUsePause">此事件是否可以被暫停，可不填，預設為不可被暫停</param>
        /// <param name="bCallBackSurplusTime">是否傳回剩餘的時間，預設為回傳剩餘時間，如果oData不為null則此值為 true </param>
        /// <returns></returns>
        public int f_RegEvent ( float fRunTime , ccCallbackV2 tccCallbackV2 ,
            float fDelayTime = 0 , ccCallbackV2 tccCallbackV2Complete = null ,
            object oData = null , bool bUsePause = false )
        {
            return this.f_RegEventForTeam( fDelayTime , fRunTime , oData , tccCallbackV2 ,
                tccCallbackV2Complete , bUsePause );
        }

        /// <summary>
        /// 至少要給予m_fSurplusTime、m_ccCallback 否則將不會執行，
        /// </summary>
        /// <param name="tsTimeEvent">給予一個sTimeEvent類別的物件
        /// 可使用ccUpdateEvent中的m_sTimeEvent去new出來</param>
        /// <returns></returns>
        public int f_RegEvent ( sTimeEvent tsTimeEvent )
        {
            return this.f_RegEventForTeam( tsTimeEvent.m_fDelayTime ,
                tsTimeEvent.m_fSurplusTime , tsTimeEvent.m_oData ,
                tsTimeEvent.m_ccCallbackV2 , tsTimeEvent.m_ccCallbackV2Complete ,
                tsTimeEvent.m_bUsePause );
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
                m_sTimeEvent.m_ccCallbackV2Complete =
                    ( ccCallbackV2 ) hashTable[ sTimeEvent.strCcCallbackComplete ];

            if ( hashTable[ sTimeEvent.strUsePause ] != null &&
                hashTable[ sTimeEvent.strUsePause ] is bool )
                m_sTimeEvent.m_bUsePause = ( bool ) hashTable[ sTimeEvent.strUsePause ];

            return this.f_RegEvent( m_sTimeEvent );
        }

        private int f_RegEventForTeam ( float fDelayTime , float fRunTime , object oData ,
            ccCallbackV2 tccCallbackV2 , ccCallbackV2 tccCallbackV2Complete , bool bUsePause  )
        {
            if ( ( tccCallbackV2 != null ) && ( fDelayTime >= 0f ) )
            {
                //額外條件
                //如果complete為空，則指定給tccCallback
                if ( tccCallbackV2Complete == null )
                    tccCallbackV2Complete = tccCallbackV2;

                sTimeEvent tClass1 = new sTimeEvent
                {
                    iId = ++iKeyId ,
                    m_fDelayTime = fDelayTime ,
                    m_fSurplusTime = fRunTime ,
                    m_oData = oData ,
                    m_ccCallbackV2 = tccCallbackV2 ,
                    m_ccCallbackV2Complete = tccCallbackV2Complete ,
                    m_bUsePause = bUsePause ,
                };
                _aTimeEventList.Add( tClass1 );

                if ( this.enabled == false )
                    this.enabled = true;
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
            int iIndex = _aTimeEventList.FindIndex( e => e.iId == iEventId );
            if ( iIndex != -1 )
            {
                this._aTimeEventList.RemoveAt( iIndex );
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
            this._aTimeEventList.Clear();
        }

        /// <summary>
        /// 取得指定ID的事件目前剩餘的秒數
        /// 找不到時，回傳float類型 數值0f
        /// </summary>
        /// <param name="iEventID"></param>
        /// <returns></returns>
        public float f_GetTimeEventExcuteCount ( int iEventID )
        {
            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _aTimeEventList != null )
                return _sTimeEvent.m_fSurplusTime;
            else
            {
                Debug.Log( "f_GetTimeEventExcuteCount 找不到此 iEventID 事件" );
                return 0f;
            }
        }

        /// <summary>
        /// 增加指定ID的事件目前秒數，可為負數，如果相加後小於等於0，則事件結束
        /// </summary>
        /// <param name="iEventID">該時間事件的ID</param>
        /// <param name="fSecond">增加的秒數(float)，可為負的(會相減)，如果相減後低於0則直接完成計時</param>
        /// <returns></returns>
        public void f_AddTimeEventExcuteCount ( int iEventID , float fSecond )
        {
            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _sTimeEvent != null )
                _sTimeEvent.m_fSurplusTime += fSecond;
            else
            {
                Debug.Log( "ccUpdateEvent : f_AddTimeEventExcuteCount 找不到此 " + iEventID + " 事件" );
            }
        }
        /// <summary>
        /// 設定指定ID的事件目前秒數，可為負數，如果設定後秒數小於等於0，則事件結束
        /// </summary>
        /// <param name="iEventID">該時間事件的ID</param>
        /// <param name="fSecond">設定的秒數(float)，可為負的，如果設定後低於0則直接完成計時</param>
        /// <returns></returns>
        public void f_SetTimeEventExcuteTime ( int iEventID , float fSecond )
        {
            sTimeEvent _sTimeEvent = _aTimeEventList.Find( e => e.iId == iEventID );
            if ( _sTimeEvent != null )
                _sTimeEvent.m_fSurplusTime = fSecond;
            else
            {
                Debug.Log( "ccTimeEvent : f_GetTimeEventExcuteTime 找不到此 " + iEventID
                   + " 事件" );
            }
        }
        private void Update ()
        {
            if ( this._aTimeEventList.Count > 0 )
            {
                for ( int i = 0 ; i < this._aTimeEventList.Count ; i++ )
                {
                    this._sTimeEvent = this._aTimeEventList[ i ];
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
                                if ( this._sTimeEvent.m_ccCallbackV2 == null )
                                {
                                    this._aWaitDelEvent.Add( this._sTimeEvent );
                                }
                                else
                                {
                                    this._sTimeEvent.m_ccCallbackV2( this._sTimeEvent.m_oData , this._sTimeEvent.m_fSurplusTime );
                                }
                            }
                            else
                            {
                                this._aWaitDelEvent.Add( this._sTimeEvent );
                                if ( this._sTimeEvent.m_ccCallbackV2Complete != null )
                                {
                                    this._sTimeEvent.m_ccCallbackV2Complete( this._sTimeEvent.m_oData , 0f );
                                }
                            }

                        }
                    }
                }
                if ( this._aWaitDelEvent.Count > 0 )
                {
                    for ( int i = 0 ; i < _aWaitDelEvent.Count ; i++ )
                    {
                        this._aTimeEventList.Remove( _aWaitDelEvent[ i ] );

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
    /// 可註冊一個搖晃物體的事件，結束時也可註冊函式等著被呼叫
    /// 註冊的物體不限為攝影機，普通遊戲物件都可
    /// </summary>
    public class ccShaking
    {
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
            return ccUpdateEvent.Instance.f_RegEvent( fShakeDuration , Shake , fDelayTime , ShakeComplete , obj , bUsePause );
        }

        private static void Shake ( object data , float fTime )
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
        private static void ShakeComplete ( object data , float fTime )
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
    public class ccCommonClass
    {
        /// <summary>
        /// ccTimeEvent與ccUpdate 中會用到的Singleton GameObject
        /// </summary>
        public static GameObject ccEngineSingletonObj;
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
