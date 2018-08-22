/*
 * 作成日：180604
 * ゲーム非同期ロード
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoad : MonoBehaviour
{
    private GameObject loadObj;//ロード表示UIOBJ
    private Slider slider;//ロード表示スライダー

    private bool isLoadStart = false;   // ロード始めたか？
    private bool isHaveLoadObj = false; // ロードオブジェはあるか？

    [SerializeField]
    private Scenes nextScene;//次のシーン
    /// <summary>
    /// 次のシーン
    /// </summary>
    public Scenes NextScene
    {
        get { return nextScene; }
        set { nextScene = value; }
    }

    /// <summary>
    /// シーン(ビルド順に合わせて)
    /// </summary>
    public enum Scenes
    {
        Title,          // タイトル
        Main,           // メインゲーム
        Result          // リザルト
    }

    void Start()
    {
        //LoadOBJがある場合取得する
        if (GameObject.Find("LoadingObj") != null)
        {
            loadObj = GameObject.Find("LoadingObj").gameObject;
            slider = loadObj.GetComponentInChildren<Slider>();
            isHaveLoadObj = true;
        }
    }

    /// <summary>
    /// ロードOBJ付きの非同期ロード
    /// </summary>
    public void LoadingStartWithOBJ()
    {
        if (!isLoadStart)
        {
            if (isHaveLoadObj)
            {
                //ロード画面UIを表示する
                loadObj.GetComponent<CanvasGroup>().alpha = 1;
                //ロードコルーチン開始
                StartCoroutine(LoadData());
                isLoadStart = true;
            }
            else
            {
                //OBJ入れてなかったら強制OBJなしの同型メソッドを使用
                LoadingStartWithoutOBJ();
            }
        }
    }

    /// <summary>
    /// ロードOBJ付きの非同期ロード(遅延付き)
    /// </summary>
    /// <param name="delayTime"></param>
    public void LoadingStartWithOBJ(float delayTime)
    {
        if (isHaveLoadObj)
        {
            Invoke("LoadingStartWithOBJ", delayTime);
        }
        else
        {
            Invoke("LoadingStartWithoutOBJ", delayTime);
        }
    }

    /// <summary>
    /// 普通にロード
    /// </summary>
    public void LoadingStartWithoutOBJ()
    {
        if (!isLoadStart)
        {
            SceneManager.LoadScene((int)nextScene);
            isLoadStart = true;
        }
    }

    /// <summary>
    /// 普通にロード(遅延付き)
    /// </summary>
    /// <param name="delayTime"></param>
    public void LoadingStartWithoutOBJ(float delayTime)
    {
        Invoke("LoadingStartWithoutOBJ", delayTime);
    }

    /// <summary>
    /// 非同期ロード
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadData()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((int)nextScene);
        async.allowSceneActivation = false;// シーン遷移をしない

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (async.progress < 0.9f)
        {
            slider.value = async.progress;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Scene Loaded");

        // ロード完了時進捗スライダーの値をMAXに
        slider.value = 1;

        yield return new WaitForSeconds(0.5f);

        async.allowSceneActivation = true;// シーン遷移許可
    }
}
