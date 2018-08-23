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
    private Player player;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        sceneState = SceneState.CheckEnd;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
                    //Idle
                    player.PlayerAnim.SetInteger("AnimIndex", 2);
                }
                else if (end == "GameOver")
                {
                    resultTextTrfm.GetComponent<SpriteRenderer>().sprite = endMats[1];
                    //AOQ_Idle
                    player.PlayerAnim.SetInteger("AnimIndex", 18);
                }
                sceneState = SceneState.SceneStart;
                break;

            case SceneState.Defalt:
                CheckIsOneMorePlay();
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

    private void CheckIsOneMorePlay()
    {
        if (eyeTracking.IsNowLookingItemSelect)
        {
            //ONEMORE? -> YES
            if (eyeTracking.NowLookingItem.GetComponent<SelectableItem>().SelectionAns == SelectableItem.SelectionAnswer.Yes)
            {
                gameLoad.NextScene = GameLoad.Scenes.Main;
                isNowSceneFinish = true;
            }
            //ONEMORE? -> NO
            else if (eyeTracking.NowLookingItem.GetComponent<SelectableItem>().SelectionAns == SelectableItem.SelectionAnswer.No)
            {
                gameLoad.NextScene = GameLoad.Scenes.Title;
                isNowSceneFinish = true;
            }
        }
    }

}
