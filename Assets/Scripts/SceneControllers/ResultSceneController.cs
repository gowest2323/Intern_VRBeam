using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リザルトシーンコントローラー
/// </summary>
public class ResultSceneController : SceneController
{
    //生きたWAVE数
    private int livedWave = 0;
    //倒した敵数
    private int killedEnemy = 0;
    //リザルトテキストOGJ
    [SerializeField]
    private Text waveResultText;
    [SerializeField]
    private Text killedEnemyText;
    [SerializeField]
    private GameObject oneMoreTextOBJ;
    [SerializeField]
    private Text rankingUpdateText;

    private bool isOneMoreTextShowed = false;

    private Player player;

    

    // Use this for initialization
    public override void Start ()
    {
        scenes = Scenes.Result;
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
                CheckPlayResult();
                SetPlayResultToRanking();
                waveResultText.text = "Wave " + livedWave.ToString("000") + " まで生き残れました！";
                killedEnemyText.text = "敵を " + killedEnemy + " 体倒しました！";

                //Idle
                player.PlayerAnim.SetInteger("AnimIndex", 2);

                sceneState = SceneState.SceneStart;
                break;

            case SceneState.Defalt:
                //if (!isOneMoreTextShowed)
                //{
                //    StartCoroutine(ShowOneMoreText());
                //    isOneMoreTextShowed = true;
                //}

                //CheckIsOneMorePlay();

                if (ranking.RANKING_STATE == Ranking.RankingState.UpdateFinish)
                {
                    if (!isOneMoreTextShowed)
                    {
                        StartCoroutine(ShowOneMoreText());
                        isOneMoreTextShowed = true;
                    }
                    CheckIsOneMorePlay();
                }
                break;
        }
    }


    /// <summary>
    /// どのWAVEまで生きたのかをチェック
    /// </summary>
    private void CheckPlayResult()
    {
        if(PlayerPrefs.GetInt(waveNumPrefsKey) != 0)
        {
            livedWave = PlayerPrefs.GetInt(waveNumPrefsKey);
            killedEnemy = PlayerPrefs.GetInt(killedEnemyPrefsKey);
        }
    }

    /// <summary>
    /// もう一回遊ぶかをチェック
    /// </summary>
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

    private void SetPlayResultToRanking()
    {
        ranking.KilledEnemy = killedEnemy;
        ranking.LivedEnemy = livedWave;
    }

    private IEnumerator ShowOneMoreText()
    {
        rankingUpdateText.text = "記録更新完了！";

        yield return new WaitForSeconds(2f);

        rankingUpdateText.enabled = false;
        oneMoreTextOBJ.SetActive(true);
    }

}
