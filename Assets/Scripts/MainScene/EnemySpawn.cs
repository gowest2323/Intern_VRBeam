using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵生成クラス
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    //生成用敵プレハブ配列
    [SerializeField]
    private GameObject[] enemyPrafbs;
    //敵生成リスト
    private List<Vector3> enemySpawnPosList;

    enum EnemyType
    {
        Normal, // ダメージ小・速度中
        Strong, // ダメージ中・速度高
        Big     // ダメージ大・速度低
    }

    //敵の生成確率(Editorで設定)
    [SerializeField]
    private int[] enemySpawnPercentage;
    //コード用の計算確率
    private List<int> enemySpawnPercentageList;


    // Use this for initialization
    void Start ()
    {
        enemySpawnPosList = new List<Vector3>();
        //木の生成位置を使う
        enemySpawnPosList = transform.parent.GetComponentInChildren<TreeSpawn>().TreePositionList;

        enemySpawnPercentageList = new List<int>();
        SetEnemySpawnPercentage();
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    /// <summary>
    /// 敵をスポーンする
    /// </summary>
    public void SpawnEnemy()
    {
        //位置リストからランダムの場所を取って
        int randomIndex = Random.Range(0, enemySpawnPosList.Count);
        //生成タイプパーセンテージ
        int enemyTypePercentage = Random.Range(0, 100);

        //Big >> Strong >> Normal
        for (int i = enemyPrafbs.Length - 1; i >= 0; i--)
        {
            //0-9 >> 10-39 >> 40-99
            if (enemyTypePercentage <= enemySpawnPercentageList[i] - 1)
            {
                //敵生成
                Instantiate(enemyPrafbs[(int)i], enemySpawnPosList[randomIndex], Quaternion.identity);
                break;
            }
        }
    }

    /// <summary>
    /// 敵の生成確率を設定
    /// </summary>
    private void SetEnemySpawnPercentage()
    {
        //60.30.10
        for(int i = 0; i < enemySpawnPercentage.Length; i++)
        {
            int tmp = 0;
            for (int j = 0; j < enemySpawnPercentage.Length; j++)
            {
                //i0-j012・i1-j12・i2-j2
                if (j < i )
                {
                    continue;
                }
                tmp += enemySpawnPercentage[j];
            }
            enemySpawnPercentageList.Add(tmp);
        }
    }
}
