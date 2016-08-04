using ccEngine;
using UnityEngine;

public class test_ccUpdateEvent : MonoBehaviour
{
    int id;
    float f;
    // Use this for initialization
    void Start ()
    {
        f = Time.timeSinceLevelLoad;
        print( f );
        id = ccUpdateEvent.Instance.f_RegEvent( 3.5f , cc );
        //ccUpdateEvent.Instance.f_RegEvent( ccUpdateEvent.m_sTimeEvent );
    }

    private void cc ( object data , float fTime )
    {
        //print( "Time : " + ( Time.timeSinceLevelLoad - f ) );
        print( data  + "   ,  " + fTime );
    }

    // Update is called once per frame
    void Update ()
    {
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            print( ccUpdateEvent.Instance.f_GetTimeEventExcuteCount( id ) );
            ccUpdateEvent.Instance.f_SetTimeEventExcuteTime( id , 5f );
            //print( ccUpdateEvent.Instance.f_GetTimeEventExcuteCount( id ) );
        }
        if ( Input.GetKeyDown( KeyCode.Escape ) )
            Application.LoadLevel( 1 );
    }
}
