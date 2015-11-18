﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerGamePanel : MonoBehaviour {
    public ServerPlayerInfo[] playerInfos;

	// Use this for initialization
	void Start () {
	
	}

    public void Reset()
    {
        foreach (ServerPlayerInfo spi in playerInfos)
        {
            if (spi != null)
            {
                spi.Reset();
                spi.gameObject.SetActive(false);
            }
        }
    }
}