using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WAVE管理クラス
/// </summary>
public class WaveController : MonoBehaviour
{
    //WAVE数
    [SerializeField]
    private int numOfWaves = 5;
    //WAVEの時間
    [SerializeField]
    private float secondsPerWave = 30.0f;

    //最初のWAVEの敵の数
    [SerializeField]
    private int originEnemyAmount = 10;
    //上乗せする敵の数
    [SerializeField]
    private int enemyPlusPerWave= 10;
    //各WAVEの敵の数
    private List<int> enemysOfWaves;

    //今のWAVE数
    [SerializeField]
    private int nowWave = 0;
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

    private EnemySpawn enemySpawn;
    private Player player;

    //UI
    [SerializeField]
    private Text waveText;
    [SerializeField]
    private Text waveCountDownText;

	// Use this for initialization
	void Awake ()
    {
        enemysOfWaves = new List<int>();
        for (int i = 0; i < numOfWaves; i++)
        {
            enemysOfWaves.Add(originEnemyAmount + i * enemyPlusPerWave);
        }

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
                waveText.text = "WAVE : " + nowWave + "/" + numOfWaves;
                waveTimer = secondsPerWave;
                break;

            case WaveState.WaveStandBy:
                nowWave++;
                if (nowWave <= numOfWaves)
                {
                    timeForSpawnEnemy = secondsPerWave / enemysOfWaves[nowWave - 1];
                    waveText.text = "WAVE : " + nowWave + "/" + numOfWaves;
                    waveTimer = secondsPerWave;
                    waveState = WaveState.WaveStart;
                }
                else
                {
                    waveState = WaveState.AllWaveFinish;
                }
                break;

            case WaveState.WaveStart:
                WaveCountDown();
                EnemySpawnPerTime(timeForSpawnEnemy);
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

        waveCountDownText.text = waveTimer.ToString("00.00");
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
            enemySpawn.SpawnEnemyInRandomPos();
            enemySpawnTimer = 0;
        }
    }
}
