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
        scenes = Scenes.Title;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void UpdateWithSceneState()
    {
        base.UpdateWithSceneState();

        switch (sceneState)
        {
            case SceneState.Defalt:
                //頭を注目して
                if(eyeTracking.IsNowLookingItemSelected &&
                   eyeTracking.NowLookingItem.GetComponent<SelectableItem>().SelectionAns == SelectableItem.SelectionAnswer.GameStart)
                {
                    isNowSceneFinish = true;
                }
                break;
        }
    }
}
