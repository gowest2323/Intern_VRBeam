﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 総合シーンコントローラー（各シーンコントローラーの継承元）
/// </summary>
public class SceneController : MonoBehaviour
{
    //画面フェードコントローラー
    [HideInInspector]
    public FadeController fadeController;
    //今のシーンが終了したか？
    [HideInInspector]
    public bool isNowSceneFinish = false;
    //ロード関連クラス
    [HideInInspector]
    public GameLoad gameLoad;
    //プレイヤーが生きたWAVE数prefsキー
    [HideInInspector]
    public string waveNumPrefsKey = "WaveNumPlayerLive";
    //倒した敵数prefsキー
    [HideInInspector]
    public string killedEnemyPrefsKey = "KilledEnemy";
    //シーン状態
    [HideInInspector]
    public SceneState sceneState = SceneState.SceneStart;
    public enum SceneState
    {
        SceneStart,  //シーン開始(フェードイン)
        Defalt,      //基準状態
        SceneEnd,    //シーン終了(フェードアウト)
        ToNextScene, //次のシーンに移行(ロード処理)
        SceneStandby //前置作業
    }
    //目線
    [HideInInspector]
    public EyeTracking eyeTracking;
    //ランキング
    [HideInInspector]
    public Ranking ranking;

    [HideInInspector]
    public Scenes scenes;
    public enum Scenes
    {
        Title,
        Main,
        Result
    }


    // Use this for initialization
    public virtual void Start ()
    {
        fadeController = GameObject.FindGameObjectWithTag("FadeController").GetComponent<FadeController>();
        gameLoad = GetComponent<GameLoad>();
        eyeTracking = GetComponent<EyeTracking>();
        ranking = GetComponent<Ranking>();
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        UpdateWithSceneState();
    }

    public virtual void UpdateWithSceneState()
    {
        switch (sceneState)
        {
            case SceneState.SceneStart:
                if (!fadeController.IsFadeInFinish)
                {
                    fadeController.FadeIn();
                }
                else
                {
                    if(scenes == Scenes.Title)
                    {
                        ranking.RANKING_STATE = Ranking.RankingState.Loading;
                    }
                    else if(scenes == Scenes.Result)
                    {
                        ranking.RANKING_STATE = Ranking.RankingState.Updating;
                    }
                    sceneState = SceneState.Defalt;
                }
                break;

            case SceneState.Defalt:
                if (isNowSceneFinish)
                {
                    sceneState = SceneState.SceneEnd;
                }
                break;

            case SceneState.SceneEnd:
                if (!fadeController.IsFadeOutFinish)
                {
                    fadeController.FadeOut();
                }
                else
                {
                    sceneState = SceneState.ToNextScene;
                }
                break;

            case SceneState.ToNextScene:
                gameLoad.LoadingStartWithOBJ();
                break;
        }
    }
}
