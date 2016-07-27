using ccEngine;
using UnityEngine;
/// <summary>
/// 測試次數
/// </summary>
public class test_ccTimeEvent : MonoBehaviour
{
    int id;
    float f;
    // Use this for initialization
    void Start ()
    {
        f = Time.timeSinceLevelLoad;
        print( f );
        id = ccTimeEvent.Instance.f_RegEvent( 3.15f , cc , false );
        print( ccTimeEvent.Instance.f_GetTimeEventExcuteTime( id ) );
        print( ccTimeEvent.Instance.f_GetTimeEventExcuteTime( 11 ) );

    }

    private void cc ( object data )
    {
        print( Time.timeSinceLevelLoad - f );
        f = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update ()
    {
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            print( ccTimeEvent.Instance.f_GetTimeEventExcuteTime( id ) );
            ccTimeEvent.Instance.f_SetTimeEventExcuteTime( id , 6f );
            print( ccTimeEvent.Instance.f_GetTimeEventExcuteTime( id ) );
        }
    }
}
