using ccEngine;
using UnityEngine;

public class TestCcTime : MonoBehaviour
{
    int iId1,iId2;
    public int t =3;
    float t1,t2;
    // Use this for initialization
    void Start ()
    {
        //print( ccTimeEvent.Instance.f_RegEvent( 2 , true , 1 , aa ) );
        //iId = ccTimeEvent.Instance.f_RegEvent( 5 , false , 3 , aa );
        //ccTimeEvent.Instance.f_RegEvent( 2 , false , 1 , aa );
        //iId1 = ccTimeEvent.Instance.f_RegEvent( 1 , true , 2 , aa , true );
        //iId2 = ccTimeEvent.Instance.f_RegEvent( 1 , true , 3 , aa );
        //iId1 = ccUpdateEvent.Instance.f_RegEvent( 2 , 5 , aa , 1 , true );
        //ccUpdateEvent.Instance.f_RegEvent( 1 , 4 , aa , 2 , false );
        //ccUpdateEvent.Instance.f_RegEvent( 0 , 5 , aa );
        //ccTimeEvent.Instance;
        //print( Time.timeSinceLevelLoad );
        //ccUpdateEvent.Instance.f_RegEvent( 0 , t , tt , tt , 3 , false  );

        //兩個攝影機各自晃動
        //iId1 = CameraShake.ShakeCamera( gameObject , t , 0 , t * 0.1f , ( CameraShake.eShakeType ) (t-1 ));

        //hashTable
        //System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
        //hashTable[ sTimeEvent.strDelay ] = "ds";
        //hashTable[ sTimeEvent.strCcCallback ] = 332;
        //hashTable[ sTimeEvent.strCcCallbackComplete ] = "2w";
        //hashTable[ sTimeEvent.strArgument ] = ( ccCallback ) tt;
        //hashTable[ sTimeEvent.strCallBackSurplusTime ] = 32;
        //hashTable[ sTimeEvent.strRuntime ] = false;
        //hashTable[ sTimeEvent.strUsePause ] = 3;
        //ccUpdateEvent.Instance.f_RegEvent( hashTable );

        //sTimeEvent
        //ccUpdateEvent.Instance.f_RegEvent( hashTable );
        //ccUpdateEvent.m_sTimeEvent = new sTimeEvent( 5 , tt  );
        //ccUpdateEvent.Instance.f_RegEvent( ccUpdateEvent.m_sTimeEvent );


    }

    private void tt ( object data )

    {
        print( data );

        t1 -= Time.deltaTime;
        //print( t );
    }

    private void aa ( object data )
    {
        print( "Speed : " + ( Time.timeSinceLevelLoad - t1 ) );
        //t1 = Time.timeSinceLevelLoad;
        //print( Time.timeSinceLevelLoad );
    }

    private void bb ( object data )
    {
        print( "Speed : " + ( Time.timeSinceLevelLoad - t2 ) );
    }
    // Update is called once per frame
    void Update ()
    {

        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            t1 = Time.timeSinceLevelLoad;
            print( t1 );
            ccTimeEvent.Instance.f_RegEvent( 2 , aa );
            ccTimeEvent.Instance.f_RegEvent( 5 , aa );
            ccTimeEvent.Instance.f_RegEvent( 30 , aa );
            ccTimeEvent.Instance.f_RegEvent( 15 , aa );

            //ccTimeEvent.Instance.f_UnRegEvent( 111 );
            //ccTimeEvent.Instance.f_UnRegEvent( iId2 );
            //ccTimeEvent.Instance.f_UnRegAllEvent();
            //ccTimeEvent.Instance.f_Pause();
            //ccUpdateEvent.Instance.f_RegEvent( 0 , 5 , false , 1 , aa , false );
            //ccUpdateEvent.Instance.f_Pause();
            //ccUpdateEvent.Instance.f_UnRegEvent( iId1 );
            //ccUpdateEvent.Instance.f_UnRegAllEvent();
            //CameraShake.ShakeCamera( gameObject , 2 , 3 , 0.2f );

        }
        if ( Input.GetKeyDown( KeyCode.Escape ) )
        {
            //ccUpdateEvent.Instance.f_RegEvent( 0 , 5 , aa , 1 , false );

            //ccTimeEvent.Instance.f_Resume();
            ccUpdateEvent.Instance.f_Resume();
        }
    }

}
