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
        Updating,
        UpdateFinish
    }

    //DBロード始まったか？
    private bool isLoadStart = false;
    //DBアップデート始まったか？終わったか？
    private bool isUpdateStart = false;
    private bool isUpdateEnd = false;

    //DBから受け取ったリザルトを格納するディクショナリ
    private Dictionary<string, int> killedEnemyRankDic = new Dictionary<string, int>();
    private Dictionary<string, int> livedWavesRankDic = new Dictionary<string, int>();

    //タイトルランキング表示テキスト
    [SerializeField]
    private Text[] killedEnemyRankText;
    [SerializeField]
    private Text[] livedWavesRankText;
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
    private Text[] selfKilledRankText;
    [SerializeField]
    private Text[] selfLivedRankText;

    //記録がない判定文
    private string noRecord_Personal = "Don't have personal record";
    private string noRecord_All = "Don't have any record";


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
                    //ランキング表示->Title
                    StartCoroutine(Show(killedEnemyRank));
                    StartCoroutine(Show(livedWavesRank));
                    isLoadStart = true;
                }
                break;
            case RankingState.Updating:
                if (!isUpdateStart)
                {
                    //ランキング更新・追加->Result
                    StartCoroutine(CheckIfRecordExist_Result(killedEnemyRank, PlayerStatus.Instance.UniqueID, killedEnemy));
                    StartCoroutine(CheckIfRecordExist_Result(livedWavesRank, PlayerStatus.Instance.UniqueID, livedWaves));
                    isUpdateStart = true;
                }

                if (isUpdateEnd)
                {
                    rankingState = RankingState.UpdateFinish;
                }
                break;
        }

    }

    /// <summary>
    /// DBに個人記録があるかをチェック(Result)
    /// </summary>
    /// <param name="ranking"></param>
    /// <param name="guid"></param>
    /// <param name="playerScore"></param>
    /// <returns></returns>
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

        //個人記録がない
        if (result.text.Contains(noRecord_Personal))
        {
            print("new record add to DB");
            //追加
            yield return StartCoroutine(Submit(ranking, guid, playerScore));
        }
        //個人記録がある
        else
        {
            var received_data = Regex.Split(result.text, ",");

            //新しい成績が以前のより高い場合
            if (playerScore > int.Parse(received_data[1]))//0->name, 1->score
            {
                print("update record");
                //更新
                yield return StartCoroutine(Update(ranking, guid, playerScore));
            }
            else
            {
                print("the record in DB is more higher.");
            }
        }

        isUpdateEnd = true;

        //更新が終わったら自分の順位をチェック
        yield return StartCoroutine(CheckRankByGuid(ranking, guid));
    }

    /// <summary>
    /// DBに個人記録があるかをチェック(Title)
    /// </summary>
    /// <param name="ranking"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
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

        //個人記録がない
        if (result.text.Contains(noRecord_Personal))
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
        //個人記録がある
        else
        {
            //自分の順位をチェック
            yield return StartCoroutine(CheckRankByGuid(ranking, guid));
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
            isUpdateEnd = true;
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
            isUpdateEnd = true;
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

        //DBに何も記録がない
        if (result.text.Contains(noRecord_All))
        {
            //表示
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

            //表示
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
    }

    /// <summary>
    /// 特定のプレイヤーの順位を探す
    /// </summary>
    /// <param name="ranking"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    private IEnumerator CheckRankByGuid(string ranking, string guid)
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

    /// <summary>
    /// PHPから受け取ったIDと点数をディクショナリに入れる
    /// </summary>
    /// <param name="records"></param>
    /// <param name="data"></param>
    /// <param name="targetDic"></param>
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
    /// <param name="rankText">ランキングテキストOBJ</param>
    private void ShowRanking(Dictionary<string, int> targetDic, Text[] rankText)
    {
        //targetDicの結果を元に順位Listを作る
        List<int> rankList = new List<int>();
        var targetList = targetDic.ToList();
        rankList = GetRankList(targetList);

        //Loading文字を透明に
        rankText[(int)RankingText.LoadText].transform.GetComponent<CanvasGroup>().alpha = 0;
        //Textに順位結果を書き出す
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



