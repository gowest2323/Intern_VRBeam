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
        Normal,
        Strong
    }

    //強い敵の生成確率
    [SerializeField]
    private int percentForSpawnStrong = 30;

    	// Use this for initialization
	void Start ()
    {
        enemySpawnPosList = new List<Vector3>();
        //木の位置を使う
        enemySpawnPosList = transform.parent.GetComponentInChildren<TreeSpawn>().TreePositionList;
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

        if (enemyTypePercentage <= percentForSpawnStrong - 1)
        {
            //設定した確率でで強い敵生成
            Instantiate(enemyPrafbs[(int)EnemyType.Strong], enemySpawnPosList[randomIndex], Quaternion.identity);
        }
        else
        {
            //ノーマル敵生成
            Instantiate(enemyPrafbs[(int)EnemyType.Normal], enemySpawnPosList[randomIndex], Quaternion.identity);
        }

    }
}
