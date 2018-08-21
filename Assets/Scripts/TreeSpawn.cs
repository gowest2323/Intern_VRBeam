using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 木の自動生成クラス
/// </summary>
public class TreeSpawn : MonoBehaviour
{
    //半径
    [SerializeField]
    private float radius = 10;
    //木の数
    [SerializeField]
    private int numOfTree = 100;
    //木の位置リスト
    private List<Vector3> treePositionList;
    public List<Vector3> TreePositionList
    {
        get { return treePositionList; }
    }
    //木のプレハブ
    [SerializeField]
    private GameObject tree;

	// Use this for initialization
	void Awake ()
    {
        treePositionList = new List<Vector3>();
        Spawn();
        MakeChildrenPositoinToCircle();
    }

    // Update is called once per frame
    void Update () {}

    private void Spawn()
    {
        for(int i = 0; i < numOfTree; i++)
        {
            Instantiate(tree, transform);
        }
    }

    private void MakeChildrenPositoinToCircle()
    {
        List<GameObject> childList = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childList.Add(child.gameObject);
        }

        //オブジェクト間の角度差
        float angleDiff = 360 / (float)numOfTree;

        //各オブジェクトを円状に配置
        for (int i = 0; i < numOfTree; i++)
        {
            Vector3 treePosition = transform.position;

            float angle = (90 - angleDiff * i) * Mathf.Deg2Rad;
            treePosition.x += radius * Mathf.Cos(angle);
            treePosition.z += radius * Mathf.Sin(angle);

            childList[i].transform.position = treePosition;
            treePositionList.Add(treePosition);
        }
    }
}
