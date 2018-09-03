using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class EyeTracking : MonoBehaviour
{
    //視線入力アイテム
    [SerializeField]
    private Transform[] itemsForLook;
    //今見ているアイテム
    private Transform nowLookingItem;
    public Transform NowLookingItem
    {
        get { return nowLookingItem; }
    }

    [SerializeField]
    private Transform selectionBar;
    [SerializeField]
    private float secondsToFill = 2.5f;

    private bool isNowLookingItemSelected = false;
    public bool IsNowLookingItemSelected
    {
        get { return isNowLookingItemSelected; }
    }

    private AudioSource audioSE;

    // Use this for initialization
    void Start ()
    {
        audioSE = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        FindNowLookingItem();
        FillSelectionBar();

        //  ゲージ満タンかつトリガーを押した
        if (selectionBar.GetComponent<Image>().fillAmount >= 1 &&
            (Input.GetKeyDown(KeyCode.A) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)))
        {
            audioSE.Play();
            isNowLookingItemSelected = true;
        }
    }

    /// <summary>
    /// 今注目しているアイテムを格納
    /// </summary>
    private void FindNowLookingItem()
    {
        foreach(var item in itemsForLook)
        {
            if (item.GetComponent<VRInteractiveItem>().IsOver)
            {
                nowLookingItem = item;
            }
        }
    }

    public void FillSelectionBar()
    {
        if (nowLookingItem == null) return;

        if (nowLookingItem.GetComponent<VRInteractiveItem>().IsOver)
        {
            selectionBar.GetComponent<Image>().fillAmount += Time.deltaTime / secondsToFill;
            if (selectionBar.GetComponent<Image>().fillAmount >= 1)
            {
                selectionBar.GetComponent<Image>().fillAmount = 1;
            }
        }
        else
        {
            selectionBar.GetComponent<Image>().fillAmount -= Time.deltaTime * 2;
            if (selectionBar.GetComponent<Image>().fillAmount <= 0)
            {
                selectionBar.GetComponent<Image>().fillAmount = 0;
                nowLookingItem = null;
            }
        }
    }

}
