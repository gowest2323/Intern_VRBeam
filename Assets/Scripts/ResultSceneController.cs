using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リザルトシーンコントローラー
/// </summary>
public class ResultSceneController : SceneController
{
    //エンディング
    private int livedWave = 0;
    //リザルトテキストOGJ
    [SerializeField]
    private Text waveResultText;
    private Player player;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        sceneState = SceneState.SceneStandby;
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
            case SceneState.SceneStandby:
                CheckLivedWave();
                waveResultText.text = "Wave " + livedWave.ToString("000") + " まで生き残れました！";
                //Idle
                player.PlayerAnim.SetInteger("AnimIndex", 2);

                sceneState = SceneState.SceneStart;
                break;

            case SceneState.Defalt:
                CheckIsOneMorePlay();
                break;
        }
    }


    /// <summary>
    /// どのWAVEまで生きたのかをチェック
    /// </summary>
    private void CheckLivedWave()
    {
        if(PlayerPrefs.GetInt(waveNumPrefsKey) != 0)
        {
            livedWave = PlayerPrefs.GetInt(waveNumPrefsKey);
            Debug.Log(livedWave);
        }
        else
        {
            Debug.Log("0 WAVE");
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
