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
    /// 位置リストからランダムの場所を取って敵をスポーンする
    /// </summary>
    public void SpawnEnemyInRandomPos()
    {
        int randomIndex = Random.Range(0, enemySpawnPosList.Count);

        Instantiate(enemyPrafbs[0], enemySpawnPosList[randomIndex], Quaternion.identity);
    }
}
