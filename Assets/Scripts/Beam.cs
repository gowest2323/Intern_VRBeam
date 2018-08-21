using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ビーム
/// </summary>
public class Beam : MonoBehaviour
{
    //目の位置
    private Transform centerEyeAnchor;

    //ビーム最大距離
    [SerializeField]
    private float maxDistance = 100.0f;

    //パーティクルシステム
    private ParticleSystem laserParticleSystem;
    public ParticleSystem LaserParticleSystem
    {
        get { return laserParticleSystem; }
        set { laserParticleSystem = value; }
    }
    private List<ParticleCollisionEvent> particleCollisionEvents;

    //ダメージプレハブ
    [SerializeField]
    private GameObject damageEffectOBJ;

    // Use this for initialization
    void Awake ()
    {
        centerEyeAnchor = transform.parent;
        laserParticleSystem = GetComponent<ParticleSystem>();
        particleCollisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    /// <summary>
    /// ビームに撃たれた場所にダメージUIを表示
    /// </summary>
    /// <param name="pos"></param>
    private void ShowDamage(Vector3 pos)
    {
        Instantiate(damageEffectOBJ, pos, Quaternion.identity);
    }

    /// <summary>
    /// パーティクル衝突
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.tag == "Ground" ||
            other.gameObject.tag == "FieldItem" )//フィールド
        {
            laserParticleSystem.GetCollisionEvents(other, particleCollisionEvents);

            foreach(var colEvent in particleCollisionEvents)
            {
                Vector3 pos = colEvent.intersection;
                ShowDamage(pos);
            }
        }
        else if(other.gameObject.tag == "Enemy")//敵に当たった
        {
            laserParticleSystem.GetCollisionEvents(other, particleCollisionEvents);

            foreach (var colEvent in particleCollisionEvents)
            {
                Vector3 pos = colEvent.intersection;
                ShowDamage(pos);
            }

            Destroy(other.gameObject);
        }
    }
}
