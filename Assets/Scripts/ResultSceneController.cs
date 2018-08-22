using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リザルトシーンコントローラー
/// </summary>
public class ResultSceneController : SceneController
{
    //エンディング
    private string end = "";
    //リザルトテキストOGJ
    [SerializeField]
    private Transform resultTextTrfm;
    [SerializeField]
    private Sprite[] endMats;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        sceneState = SceneState.CheckEnd;
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
		
	}

    public override void UpdateWithSceneState()
    {
        base.UpdateWithSceneState();

        switch (sceneState)
        {
            case SceneState.CheckEnd:
                CheckEnd();
                //Endに沿って演出を変更
                if (end == "GameClear")
                {
                    resultTextTrfm.GetComponent<SpriteRenderer>().sprite = endMats[0];
                }
                else if (end == "GameOver")
                {
                    resultTextTrfm.GetComponent<SpriteRenderer>().sprite = endMats[1];
                }
                sceneState = SceneState.SceneStart;
                break;

            case SceneState.Defalt:
                //Endに沿って演出を変更
                if(end == "GameClear")
                {
                }
                else if(end == "GameOver")
                {
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isNowSceneFinish = true;
                }
                break;
        }
    }


    /// <summary>
    /// どのエンドなのかをチェック
    /// </summary>
    private void CheckEnd()
    {
        if(PlayerPrefs.GetString(endingPrefsKey) != "")
        {
            end = PlayerPrefs.GetString(endingPrefsKey);
            Debug.Log(end);
        }
        else
        {
            Debug.Log("エンドが保存してない");
        }
    }

}
