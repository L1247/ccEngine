using ccEngine;
using UnityEngine;

public class TestCameraShake : MonoBehaviour {
    public float t = 5;
	// Use this for initialization
	void Start () {
    }

    private void Complete ( object data )
    {
        print( data );
    }

    // Update is called once per frame
    void Update () {
        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            //兩個攝影機各自晃動
            ccTimeEvent.Instance.f_RegEvent( 1 , cc );
            ccTimeEvent.Instance.f_RegEvent( 1 , cc );
            ccShaking.ShakeGameObject( gameObject , t , t * 0.5f , t * 0.1f , ( eShakeType ) ( t - 1 ) , Complete );
        }
    }

    private void cc ( object data )
    {
        print( "cc" );
    }
}
