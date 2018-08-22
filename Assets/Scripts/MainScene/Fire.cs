using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    //表示継続時間
    [SerializeField]
    private float liveTime = 5.0f;
    //Destroyメソッド呼んだか？
    private bool isCallDestory = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        DestroySelfWhenTimeUp();
    }

    private void DestroySelfWhenTimeUp()
    {
        if (!isCallDestory)
        {
            Destroy(this.gameObject, liveTime);
            isCallDestory = true;
        }
    }
}
