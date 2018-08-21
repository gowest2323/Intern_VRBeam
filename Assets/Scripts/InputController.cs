using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仮のタイトルシーンのInput処理クラス
/// </summary>
public class InputController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {


    }
	
	// Update is called once per frame
	void Update ()
    {
        //トリガーをおしたら
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            //タイトルシーンの処理
        }
    }
}
