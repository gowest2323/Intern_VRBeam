using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WAVE管理クラス
/// </summary>
public class WaveController : MonoBehaviour
{
    //WAVEの時間
    [SerializeField]
    private float secondsPerWave = 30.0f;

    //最初のWAVEの敵の数
    [SerializeField]
    private int originEnemyAmount = 10;
    //上乗せする敵の数
    [SerializeField]
    private int enemyPlusPerWave= 10;

    [Header("以下表示だけ")]
    //今のWAVE数
    [SerializeField]
    private int nowWave = 0;
    public int NowWave
    {
        get { return nowWave; }
    }
    //WAVEタイマー
    [SerializeField]
    private float waveTimer = 0;
    //敵生成の時間間隔
    [SerializeField]
    private float timeForSpawnEnemy = 0f;
    //生成タイマー
    [SerializeField]
    private float enemySpawnTimer = 0;

    //WAVE状態
    public enum WaveState
    {
        Waiting,
        WaveStandBy,
        WaveStart,
        AllWaveFinish
    }
    private WaveState waveState = WaveState.Waiting;
    public WaveState NowWaveState
    {
        get { return waveState; }
        set { waveState = value; }
    }

    //プレイヤー
    private Player player;
    //敵生成
    private EnemySpawn enemySpawn;

    [Header("UI")]
    //UI
    [SerializeField]
    private Text[] waveTextGroup;
    [SerializeField]
    private Text[] waveCountDownTextGroup;
    [SerializeField]
    private Text[] numOfKilledEnemyTextGroup;

	// Use this for initialization
	void Awake ()
    {
        enemySpawn = GetComponentInChildren<EnemySpawn>();
        waveTimer = secondsPerWave;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update ()
    {
        switch (waveState)
        {
            case WaveState.Waiting:
                SetWaveText();
                waveTimer = secondsPerWave;
                break;

            case WaveState.WaveStandBy:
                nowWave++;

                timeForSpawnEnemy = secondsPerWave / (originEnemyAmount + (nowWave - 1) * enemyPlusPerWave);
                SetWaveText();
                waveTimer = secondsPerWave;
                waveState = WaveState.WaveStart;

                break;

            case WaveState.WaveStart:
                WaveCountDown();
                EnemySpawnPerTime(timeForSpawnEnemy);
                SetKilledEnemyText();
                break;

            case WaveState.AllWaveFinish:
                //ここで処理をシーンコントローラーに移す
                break;
        }
	}

    /// <summary>
    /// WAVEカウントダウン
    /// </summary>
    private void WaveCountDown()
    {
        waveTimer -= Time.deltaTime;

        if(player.Hp <= 0)
        {
            waveState = WaveState.AllWaveFinish;
        }

        if (waveTimer <= 0)
        {
            waveTimer = 0;
            waveState = WaveState.WaveStandBy;
        }

        SetWaveCountDownText();
    }

    /// <summary>
    /// 渡された時間間隔で敵を生成
    /// </summary>
    /// <param name="spawnTime"></param>
    private void EnemySpawnPerTime(float spawnTime)
    {
        enemySpawnTimer += Time.deltaTime;
        if(enemySpawnTimer >= spawnTime)
        {
            enemySpawn.SpawnEnemy();
            enemySpawnTimer = 0;
        }
    }

    private void SetWaveText()
    {
        foreach (var waveText in waveTextGroup)
        {
            waveText.text = "WAVE : " + nowWave.ToString("000");
        }
    }

    private void SetWaveCountDownText()
    {
        foreach(var waveCountDownText in waveCountDownTextGroup)
        {
            waveCountDownText.text = waveTimer.ToString("00.00");
        }
    }

    private void SetKilledEnemyText()
    {
        foreach (var killedEnemyText in numOfKilledEnemyTextGroup)
        {
            killedEnemyText.text = "Killed : " + player.NumOfKilledEnemy.ToString("000");
        }
    }
}
