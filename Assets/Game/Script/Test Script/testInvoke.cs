using UnityEngine;

public class testInvoke : MonoBehaviour
{
    float f;
    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 60;
    }

    private void cc ( object data )
    {
        print( 1f - ( Time.realtimeSinceStartup - f ) );
        ////print( Time.time );
        //long f1 = ( long ) data;
        //print( f1+ "   : " + Time.realtimeSinceStartup );

    }

    // Update is called once per frame
    void Update ()
    {
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            f = Time.realtimeSinceStartup;
            print( "start : " + f );
            ccEngine.ccTimeEvent.Instance.f_RegEvent( 50f ,  cc );
            //print("Space : " + Time.realtimeSinceStartup );
            //ccEngine.ccTimeEvent.Instance.f_RegEvent( 0.45f , false , cc );
            //ccEngine.ccTimeEvent.Instance.f_RegEvent( 0.5f , false , cc );
        }
    }
}
