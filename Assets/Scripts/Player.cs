using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

/// <summary>
/// プレイヤークラス
/// </summary>
public class Player : MonoBehaviour
{
    //HP
    [SerializeField]
    private int hp = 100;
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    //敵に当たった時のダメージ
    [SerializeField]
    private int damegeValue = 5;

    //最大エネルギー
    [SerializeField]
    private float maxEnergy = 100;
    //今のエネルギー
    [SerializeField]
    private float energy = 0;

    //ビーム
    [SerializeField]
    private Beam laserBeam;
    //発射できるか？
    [SerializeField]
    private bool isShootable = false;
    //止まっているか？
    private bool isLaserStoped = true;

    //アニメーター
    private Animator animator;

    //UI
    [SerializeField]
    private Slider hpBar;
    [SerializeField]
    private Slider energyBar;

    void Awake()
    {
        animator = transform.GetComponent<Animator>();

        if(SceneManager.GetActiveScene().name == "Title")
        {
            //Idle
            animator.SetInteger("AnimIndex", 2);
        }

        energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = hp;
        energyBar.value = energy;


        isShootable = energy <= 0 ? false : true;

        LaserBeam();
    }

    public void Damage()
    {
        hp -= damegeValue;
    }

    public void LaserBeam()
    {
        if (isShootable)
        {
            //トリガーをおしたら
            if (Input.GetKey(KeyCode.Space)/*OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)*/)
            {
                ShootBeam();
            }
            else
            {
                StopBeam();
            }
        }
        else
        {
            StopBeam();
        }
    }

    /// <summary>
    /// ビーム発射
    /// </summary>
    private void ShootBeam()
    {
        //ビームパーティクルシステムを起動
        laserBeam.LaserParticleSystem.Play();
        //エネルギー消費
        energy -= Time.deltaTime * 10;
        if (energy <= 0)
        {
            energy = 0;
            isShootable = false;
        }
        //ビーム止まっていない
        isLaserStoped = false;
    }

    /// <summary>
    /// ビーム停止
    /// </summary>
    private void StopBeam()
    {
        if (!isLaserStoped)
        {
            //ビームパーティクルシステムを停止
            laserBeam.LaserParticleSystem.Stop();
            isLaserStoped = true;
        }
        else
        {
            //ボタン押してない時
            if (!Input.GetKey(KeyCode.Space)/*OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)*/)
            {
                //エネルギー補給
                energy += Time.deltaTime * 5;
                if (energy >= maxEnergy)
                {
                    energy = maxEnergy;
                }
            }
        }
    }
}
