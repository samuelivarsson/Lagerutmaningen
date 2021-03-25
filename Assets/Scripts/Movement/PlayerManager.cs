﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    Vector3 startPos = new Vector3(10.5f, 1.5f, 12.5f);
    public static PlayerController myPlayerController;
 
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    
    
    void Start()
    {
       if(PV.IsMine)
       {
           CreateController();
       } 
    }

    void CreateController()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player", "PlayerController"), startPos, Quaternion.identity);
        myPlayerController = playerObj.GetComponent<PlayerController>();
    }
}
