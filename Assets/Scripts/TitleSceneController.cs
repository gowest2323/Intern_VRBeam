using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルシーンコントローラー
/// </summary>
public class TitleSceneController : SceneController
{
    // Use this for initialization
    public override void Start()
    {
        base.Start();
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
                //頭を注目して

                if (Input.GetKeyDown(KeyCode.P))
                {
                    isNowSceneFinish = true;
                }
                break;

            case SceneState.SceneEnd:
                //たぶん飛び込みながらフェードアウト
                break;
        }

        base.UpdateWithSceneState();
    }
}
