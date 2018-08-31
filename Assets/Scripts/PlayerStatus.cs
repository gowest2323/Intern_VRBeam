using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {


    public string UniqueID
    {
        get
        {
            if (PlayerPrefs.HasKey("guid") == false)
            {
                PlayerPrefs.SetString("guid", Guid.NewGuid().ToString());
            }
            return PlayerPrefs.GetString("guid");
        }
    }

    private static PlayerStatus pInstance;
    public static PlayerStatus Instance
    {
        get
        {
            if (pInstance == null) 
            {
                pInstance = FindObjectOfType<PlayerStatus>();
                if (pInstance == null) 
                {
                    var obj = new GameObject(typeof(PlayerStatus).Name);
                    pInstance = obj.AddComponent<PlayerStatus>();
                }
            }
            return pInstance;
        }
    }

    void Awake()
    {
        if (this == Instance)
        {
            DontDestroyOnLoad(Instance);
            return;
        }
        Destroy(gameObject);
    }

}
