using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインゲームシーンコントローラー
/// </summary>
public class MainSceneController : SceneController
{
    //Player
    private Player player;
    //Waveコントローラー
    private WaveController waveController;

    private AudioSource audioSource;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        waveController = GetComponent<WaveController>();
        audioSource = GetComponent<AudioSource>();

        //エンディング初期化
        if (PlayerPrefs.HasKey(endingPrefsKey))
        {
            PlayerPrefs.DeleteKey(endingPrefsKey);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void UpdateWithSceneState()
    {
        switch (sceneState)
        {
            case SceneState.Defalt:
                if (waveController.NowWaveState == WaveController.WaveState.Waiting)
                {
                    audioSource.Play();
                    waveController.NowWaveState = WaveController.WaveState.WaveStandBy;
                }

                if (player.Hp <= 0)
                {
                    PlayerPrefs.SetString(endingPrefsKey, "GameOver");
                    isNowSceneFinish = true;
                }
                else
                {
                    if(waveController.NowWaveState == WaveController.WaveState.AllWaveFinish)
                    {
                        PlayerPrefs.SetString(endingPrefsKey, "GameClear");
                        isNowSceneFinish = true;
                    }
                }
                break;
        }

        base.UpdateWithSceneState();
    }
}
