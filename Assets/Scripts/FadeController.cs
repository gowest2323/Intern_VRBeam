/*
 * 作成日：180504
 * フェードコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private Image fadeImage; //フェード用イメージコンポーネント
    private float fadeAlphaValue = 255; //一時計算用アルファ値

    [SerializeField]
    private float timeForFade = 1f;
    /// <summary>
    /// フェード時間
    /// </summary>
    public float FadeSeconds
    {
        get { return timeForFade; }
        set { timeForFade = value; }
    }

    //フェードイン関連
    bool isFadeInFinish = false;
    /// <summary>
    /// フェードイン終わったか？
    /// </summary>
    public bool IsFadeInFinish
    {
        get { return isFadeInFinish; }
    }
    
    //フェードアウト関連
    private bool isFadeOutFinish = false;
    /// <summary>
    /// フェードアウト終わったか？
    /// </summary>
    public bool IsFadeOutFinish
    {
        get { return isFadeOutFinish; }
    }

	// Use this for initialization
	void Start ()
    {
        //Imageコンポーネント取得
        fadeImage = this.GetComponent<Image>();
        //初期色を黒に設定
        fadeImage.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 255);
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    public void FadeIn()
    {
        //フェードイン終わってなかったら
        if (!isFadeInFinish)
        {
            //アルファ値減算
            fadeAlphaValue -= (255 / timeForFade * 60f) * (Time.deltaTime / 60f);

            //代入
            fadeImage.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      fadeAlphaValue / 255f);
            if (fadeAlphaValue <= 0)
            {
                fadeAlphaValue = 0;
                //フェードイン終わった
                isFadeInFinish = true;
            }
        }
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        //フェードアウト終わってなかったら
        if (!isFadeOutFinish)
        {
            //アルファ値加算
            fadeAlphaValue += (255 / timeForFade * 60f) * (Time.deltaTime / 60f);
            //代入
            fadeImage.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      fadeAlphaValue / 255f);
            if (fadeAlphaValue >= 255)
            {
                fadeAlphaValue = 255;
                //フェードアウト終わった
                isFadeOutFinish = true;
            }
        }
    }
}
