using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵クラス
/// </summary>
public class Enemy : MonoBehaviour
{
    //プレイヤー
    private Transform player;
    //NavMeshAgent
    private NavMeshAgent navMeshAgent;
    //Damage
    [SerializeField]
    private int damageToPlayer = 5;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

	// Update is called once per frame
	void Update ()
    {
        MoveToPlayerPosition();
    }

    /// <summary>
    /// プレイヤーの元に行く
    /// </summary>
    private void MoveToPlayerPosition()
    {
        navMeshAgent.destination = player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Playerに当たったら
        if (other.transform.tag == "Player")
        {
            //PlayerのHPを減らす
            other.transform.GetComponent<Player>().Damage(damageToPlayer);
            //自機を削除
            Destroy(this.gameObject);
        }
    }
}
