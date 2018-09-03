using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ビーム
/// </summary>
public class Beam : MonoBehaviour
{
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
    private void SpawnFire(Vector3 pos)
    {
        //生成しすぎないように制限する
        Collider[] fires = Physics.OverlapSphere(pos, 0.2f, 1 << LayerMask.NameToLayer("Fire"));

        if(fires.Length <= 0)
        {
            Instantiate(damageEffectOBJ, pos, Quaternion.identity);
        }
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
                SpawnFire(pos);
            }
        }
        else if(other.gameObject.tag == "Enemy")//敵に当たった
        {
            laserParticleSystem.GetCollisionEvents(other, particleCollisionEvents);

            foreach (var colEvent in particleCollisionEvents)
            {
                Vector3 pos = colEvent.intersection;
                SpawnFire(pos);
            }
        }
    }
}
