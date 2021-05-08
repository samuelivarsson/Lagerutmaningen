﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerName : MonoBehaviour
{
    [SerializeField] Font font;
    [SerializeField] Transform head;

    Camera mainCamera;
    PhotonView PV;

    Vector3 screenPos;
    
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
        screenPos = mainCamera.WorldToScreenPoint(head.position);
    }

    void OnGUI()
    {
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.font = font;
        GUI.color = PV.IsMine ? Color.green : Color.black;
        GUI.Label(new Rect(screenPos.x-50, Screen.height - (screenPos.y + Screen.height*0.045f), 100, Screen.height*0.1f), PV.Owner.NickName, centeredStyle);
    }
}
