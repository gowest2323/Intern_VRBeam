using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    //接続PHP
    private string url = "http://54.178.244.12/VRBeam/Ranking.php";

    //php用のランキング条件分け
    private string killedEnemyRank = "KilledEnemyRanking";
    private string livedWavesRank = "LivedWavesRanking";

    //ランキング表示人数
    [SerializeField]
    private int numForShowingRank = 5;

    //ランキング状態
    private RankingState rankingState = RankingState.Standby;
    public RankingState RANKING_STATE
    {
        get { return rankingState; }
        set { rankingState = value; }
    }
    public enum RankingState
    {
        Standby,
        Loading,
        Publishing,
        Finish
    }

    //コルーチン始まったか？
    private bool isLoadStart = false;
    private bool isPublishStart = false;

    //格納ディクショナリ
    private Dictionary<string, int> killedEnemyRankDic = new Dictionary<string, int>();
    private Dictionary<string, int> livedWavesRankDic = new Dictionary<string, int>();

    //タイトルランキング表示テキスト
    [SerializeField]
    private Text[] killedEnemyRankText, livedWavesRankText;
    //ランキングテキスト項目
    enum RankingText
    {
        Rank,
        Score,
        LoadText
    }

    //プレイ結果
    private int killedEnemy = 0;
    public int KilledEnemy
    {
        get { return killedEnemy; }
        set { killedEnemy = value; }
    }
    private int livedWaves = 0;
    public int LivedEnemy
    {
        get { return livedWaves; }
        set { livedWaves = value; }
    }

    //自分の順位テキスト
    [SerializeField]
    private Text[] selfKilledRankText, selfLivedRankText;



    // Use this for initialization
    void Start()
    {
        if (killedEnemyRankText.Length != 0)
        {
            InitRankText(killedEnemyRankText, livedWavesRankText);
        }

        if(selfKilledRankText.Length != 0)
        {
            InitRankText(selfKilledRankText, selfLivedRankText);
        }
    }

    private void InitRankText(Text[] killedRank, Text[] livedRank)
    {
        foreach (var text in killedRank)
        {
            text.text = "";
        }
        foreach (var text in livedRank)
        {
            text.text = "";
        }
        killedRank[(int)RankingText.LoadText].text = "...Loading...";
        livedRank[(int)RankingText.LoadText].text = "...Loading...";
    }

    private void Update()
    {
        switch (rankingState)
        {
            case RankingState.Loading:
                if (!isLoadStart)
                {
                    //ランキング表示
                    StartCoroutine(Show(killedEnemyRank));
                    StartCoroutine(Show(livedWavesRank));
                    isLoadStart = true;
                }
                break;
            case RankingState.Publishing:
                if (!isPublishStart)
                {
                    StartCoroutine(CheckIfRecordExist_Result(killedEnemyRank, PlayerStatus.Instance.UniqueID, killedEnemy));
                    StartCoroutine(CheckIfRecordExist_Result(livedWavesRank, PlayerStatus.Instance.UniqueID, livedWaves));
                    isPublishStart = true;
                }
                break;
        }

    }


    private IEnumerator CheckIfRecordExist_Result(string ranking, string guid, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);
        form.AddField("action", "checkIfRecordExist");
        form.AddField("guid", guid);

        WWW result = new WWW(url, form);
        yield return result;

        if (result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }

        print(result.text);
        //記録がない
        if (result.text.Contains("Don't have personal record"))
        {
            //追加
            print("new record add to DB");

            yield return StartCoroutine(Submit(ranking, guid, playerScore));
        }
        else
        {
            var received_data = Regex.Split(result.text, ",");

            //新しい成績が以前のより高い場合
            if (playerScore > int.Parse(received_data[1]))//0->name, 1->score
            {
                //更新
                print("update record");
                yield return StartCoroutine(Update(ranking, guid, playerScore));
            }
            else
            {
                print("the record in DB is more higher.");
            }
        }

        //更新が終わったら自分の順位をチェック
        yield return StartCoroutine(CheckSelfRank(ranking, guid));

        rankingState = RankingState.Finish;
    }

    private IEnumerator CheckIfRecordExist_Title(string ranking, string guid)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);
        form.AddField("action", "checkIfRecordExist");
        form.AddField("guid", guid);

        WWW result = new WWW(url, form);
        yield return result;

        if (result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }

        //記録がない
        if (result.text.Contains("Don't have personal record"))
        {
            switch (ranking)
            {
                case "KilledEnemyRanking":
                    selfKilledRankText[(int)RankingText.LoadText].text = "個人記録ありません";
                    break;

                case "LivedWavesRanking":
                    selfLivedRankText[(int)RankingText.LoadText].text = "個人記録ありません";
                    break;
            }
        }
        else
        {
            //自分の順位をチェック
            yield return StartCoroutine(CheckSelfRank(ranking, guid));

        }
    }

    /// <summary>
    /// ランキング追加
    /// </summary>
    /// <param name="ranking">ランキング種類</param>
    /// <param name="guid">名前</param>
    /// <param name="playerScore">スコア</param>
    /// <returns></returns>
    private IEnumerator Submit(string ranking, string guid, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);
        form.AddField("action", "submit");
        form.AddField("guid", guid);
        form.AddField("score", playerScore);

        WWW result = new WWW(url, form);
        yield return result;

        if (result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }
        else
        {
            print(result.text);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="ranking"></param>
    /// <param name="guid"></param>
    /// <param name="playerScore"></param>
    /// <returns></returns>
    private IEnumerator Update(string ranking, string guid, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);
        form.AddField("action", "update");
        form.AddField("guid", guid);
        form.AddField("score", playerScore);

        WWW result = new WWW(url, form);
        yield return result;

        if (result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }
        else
        {
            print(result.text);
        }
    }


    /// <summary>
    /// ランキング表示
    /// </summary>
    /// <param name="ranking">ランキング種類</param>
    /// <returns></returns>
    private IEnumerator Show(string ranking)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);//ランキング種類
        form.AddField("action", "show");//アクション
        form.AddField("limit", numForShowingRank);//上位何名まで

        WWW result = new WWW(url, form);
        yield return result;

        if(result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }

        //記録がない
        if (result.text.Contains("Don't have any record"))
        {
            switch (ranking)
            {
                case "KilledEnemyRanking":
                    killedEnemyRankText[(int)RankingText.LoadText].text = "ランキングありません";
                    selfKilledRankText[(int)RankingText.LoadText].text = "個人記録ありません";
                    break;

                case "LivedWavesRanking":
                    livedWavesRankText[(int)RankingText.LoadText].text = "ランキングありません";
                    selfLivedRankText[(int)RankingText.LoadText].text = "個人記録ありません";
                    break;
            }
        }
        else
        {
            var received_data = Regex.Split(result.text, ",");
            //内容の行数
            int records = (received_data.Length - 1) / 2;

            switch (ranking)
            {
                case "KilledEnemyRanking":
                    SetRankDic(records, received_data, killedEnemyRankDic);
                    ShowRanking(killedEnemyRankDic, killedEnemyRankText);
                    break;

                case "LivedWavesRanking":
                    SetRankDic(records, received_data, livedWavesRankDic);
                    ShowRanking(livedWavesRankDic, livedWavesRankText);
                    break;
            }

            //自分の順位をチェック
            yield return StartCoroutine(CheckIfRecordExist_Title(ranking, PlayerStatus.Instance.UniqueID));
        }


        //rankingState = RankingState.Finish;
    }

    /// <summary>
    /// 特定のプレイヤーの順位を探す
    /// </summary>
    /// <param name="ranking"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    private IEnumerator CheckSelfRank(string ranking, string guid)
    {
        WWWForm form = new WWWForm();
        form.AddField("ranking", ranking);//ランキング種類
        form.AddField("action", "checkSelfRank");
        form.AddField("guid", guid);

        WWW result = new WWW(url, form);
        yield return result;

        if (result.error != null)
        {
            Debug.Log(result.error);
            yield break;
        }

        var received_data = Regex.Split(result.text, ",");

        //自分の成績を表示
        switch (ranking)
        {
            case "KilledEnemyRanking":
                selfKilledRankText[(int)RankingText.LoadText].transform.GetComponent<CanvasGroup>().alpha = 0;
                SetRankText(selfKilledRankText, int.Parse(received_data[0]), int.Parse(received_data[2]));
                break;

            case "LivedWavesRanking":
                selfLivedRankText[(int)RankingText.LoadText].transform.GetComponent<CanvasGroup>().alpha = 0;
                SetRankText(selfLivedRankText, int.Parse(received_data[0]), int.Parse(received_data[2]));
                break;
        }
    }

    private void SetRankDic(int records, string[] data, Dictionary<string, int> targetDic)
    {
        for (int i = 0; i < records; i++)
        {
            targetDic.Add(data[2 * i], int.Parse(data[2 * i + 1]));
        }
    }

    /// <summary>
    /// ソート＆ランキング結果表示
    /// </summary>
    /// <param name="targetDic">ランキングディクショナリ</param>
    /// <param name="rankText">ランキングテキスト</param>
    private void ShowRanking(Dictionary<string, int> targetDic, Text[] rankText)
    {
        //ソート結果をリスト化
        List<int> rankList = new List<int>();
        var targetList = targetDic.ToList();
        rankList = GetRankList(targetList);

        rankText[(int)RankingText.LoadText].transform.GetComponent<CanvasGroup>().alpha = 0;
        //書き出す
        for (int i = 0; i < targetList.Count; i++)
        {
            SetRankText(rankText, rankList[i], targetList[i].Value);
        }
    }

    /// <summary>
    /// 順位を計算しリスト化
    /// </summary>
    /// <param name="sortedDic">ソートされた上位リスト</param>
    /// <returns>順位リスト</returns>
    private List<int> GetRankList(List<KeyValuePair<string, int>> sortedDic)
    {
        List<int> ranks = new List<int>();//順位

        int rank = 0;//順位
        int count = 0;//順位カウント
        float temp = 0;//スコア一時格納
        //順位付け
        for (int i = 0; i < sortedDic.Count; i++)
        {
            if (i == 0)//playerRankの一番目でしたら
            {
                temp = sortedDic[0].Value;//点数一時格納
                rank = count = 1;//一位とする・該当する順位の数=1
            }
            else//それ以降の順番
            {
                if (temp == sortedDic[i].Value)//同点でしたら
                {
                    count++;//該当する順位の人数+1
                }
                else
                {
                    temp = sortedDic[i].Value;//それ以下でしたら
                    rank += count;//前の順位＋その順位の人数=次の順位
                    count = 1;//該当する順位の人数=1
                }
            }
            ranks.Add(rank);//プレイヤー順位を格納
        }
        return ranks;
    }

    /// <summary>
    /// テキストOBJにランキング結果を入れる
    /// </summary>
    /// <param name="rankText">テキストOBJ</param>
    /// <param name="ranks">順位</param>
    /// <param name="guid">名前</param>
    /// <param name="score">スコア</param>
    private void SetRankText(Text[] rankText, int ranks, int score)
    {
        //順位
        rankText[(int)RankingText.Rank].text += ranks + "位\n";
        //スコア
        if (rankText[0].transform.parent.name.Contains("Enemy"))
        {
            rankText[(int)RankingText.Score].text += score + " Killed\n";
        }
        else
        {
            rankText[(int)RankingText.Score].text += score + " Lived\n";
        }
    }





}



