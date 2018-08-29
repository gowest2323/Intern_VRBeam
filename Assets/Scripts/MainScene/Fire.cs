using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    //表示継続時間
    [SerializeField]
    private float liveTime = 2.0f;
    //Destroyメソッド呼んだか？
    private bool isCallDestory = false;

    [SerializeField]
    private AudioClip[] explosionSE;

    private Player player;
    private AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
        DestroySelfWhenTimeUp();
    }

    private void DestroySelfWhenTimeUp()
    {
        if (!isCallDestory)
        {
            Destroy(this.gameObject, liveTime);
            isCallDestory = true;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Enemy")//敵に当たった
        {
            int randomSEIndex = Random.Range(0, explosionSE.Length);
            audioSource.PlayOneShot(explosionSE[randomSEIndex]);

            player.AddKilledEnemy();
            Destroy(other.gameObject);
        }
    }
}
