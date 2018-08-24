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
    //Player状態
    [SerializeField]
    private PlayerState playerState = PlayerState.ModelMode;
    enum PlayerState
    {
        GameMode,
        ModelMode
    }

    //HP
    [SerializeField]
    private int hp = 100;
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    //最大エネルギー
    [SerializeField]
    private float maxEnergy = 100;
    //今のエネルギー
    [SerializeField]
    private float energy = 0;

    //ビーム
    [SerializeField]
    private Beam beam;
    //発射できるか？
    [SerializeField]
    private bool isShootable = false;
    //止まっているか？
    private bool isLaserStoped = true;

    //アニメーター
    private Animator animator;
    public Animator PlayerAnim
    {
        get { return animator; }
        set { animator = value; }
    }

    //UI
    [SerializeField]
    private Slider hpBar;
    [SerializeField]
    private Slider energyBar;

    //MainCamera
    [SerializeField]
    private Transform cameraRig;
    [SerializeField]
    private Transform centerEye;

    void Awake()
    {
        animator = transform.GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name == "Title")
        {
            //Idle
            animator.SetInteger("AnimIndex", 2);
        }

        energy = maxEnergy;
    }


    float angle = 0.0F;
    Vector3 axis = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.GameMode:
                RotateBody();

                hpBar.value = hp;
                energyBar.value = energy;

                isShootable = energy <= 0 ? false : true;

                LaserBeam();
                break;

            case PlayerState.ModelMode:
                break;
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
    }

    private void LaserBeam()
    {
        if (isShootable)
        {
            //トリガーをおしたら
            if (/*Input.GetKey(KeyCode.Space) ||*/ OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))//トリガー長押し
            {
                ShootBeam();
            }
            else
            {
                StopAndChargeBeam();
            }
        }
        else
        {
            StopAndChargeBeam();
        }
    }

    /// <summary>
    /// ビーム発射
    /// </summary>
    private void ShootBeam()
    {
        //ビームパーティクルシステムを起動
        beam.LaserParticleSystem.Play();

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
    private void StopAndChargeBeam()
    {
        if (!isLaserStoped)
        {
            //ビームパーティクルシステムを停止
            beam.LaserParticleSystem.Stop();
            isLaserStoped = true;
        }
        else
        {
#if UNITY_EDITOR
            if (!Input.GetKey(KeyCode.Space))
            {
                //エネルギー補給
                energy += Time.deltaTime * 5;
                if (energy >= maxEnergy)
                {
                    energy = maxEnergy;
                }
            }
#endif

            //ボタン押してない時
            if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
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

    private void RotateBody()
    {
        transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);
    }
}
