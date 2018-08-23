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

    private AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
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
            audioSource.Play();
            Destroy(other.gameObject);
        }
    }
}
