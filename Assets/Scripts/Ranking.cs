using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class Ranking : MonoBehaviour
{
    private string url = "http://54.178.244.12/VRBeam/Ranking.php";

    private string killedEnemyRank = "killedEnemy";
    private string livedWavesRank = "livedWaves";

    private RankStatus rankStatus = RankStatus.Standby;
    public RankStatus RANK_STATUS
    {
        get { return rankStatus; }
        set { rankStatus = value; }
    }

    public enum RankStatus
    {
        Standby,
        Loading,
        Finish
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Show(killedEnemyRank));
    }

    private void Update()
    {
        switch (rankStatus)
        {
            case RankStatus.Loading:
                StartCoroutine(Show(killedEnemyRank));
                rankStatus = RankStatus.Finish;
                break;
        }

    }

    /// <summary>
    /// 追加
    /// </summary>
    /// <param name="rankMode"></param>
    /// <param name="playerName"></param>
    /// <param name="playerScore"></param>
    /// <returns></returns>
    private IEnumerator Submit(string rankMode, string playerName, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("mode", rankMode);
        form.AddField("action", "submit");
        form.AddField("name", playerName);
        form.AddField(rankMode, playerScore);

        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);
    } 

    private IEnumerator DeleteAll(string rankMode)
    {
        WWWForm form = new WWWForm();
        form.AddField("mode", rankMode);
        form.AddField("action", "delete_all");

        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);
    }

    IEnumerator Delete(string rankMode, string playerName)
    {
        WWWForm form = new WWWForm();
        form.AddField("mode", rankMode);
        form.AddField("action", "delete");
        form.AddField("name", playerName);

        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);

    }

    IEnumerator Update(string rankMode, string playerName, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("mode", rankMode);
        form.AddField("action", "update");
        form.AddField("name", playerName);
        form.AddField(rankMode, playerScore);

        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);
    }


    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="rankMode"></param>
    /// <returns></returns>
    private IEnumerator Show(string rankMode)
    {
        WWWForm form = new WWWForm();
        form.AddField("mode", rankMode);
        form.AddField("action", "show");

        WWW result = new WWW(url, form);
        yield return result;

        if(result.error != null)
        {
            Debug.Log(result.error);
        }

        var received_data = Regex.Split(result.text, ",");
        int records = (received_data.Length - 1) / 2;

        for (int i = 0; i < records; i++)
        {
            print("Name: " + received_data[2 * i] + " Score: " + received_data[2 * i + 1]);
        }
    }

}



