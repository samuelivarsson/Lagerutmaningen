﻿using UnityEngine;
using Photon.Pun;

public class ObjectTriggerCheck : MonoBehaviour
{
    [SerializeField] Material standardTile;
    [SerializeField] Material standardDropZone;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (PV.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;
        
        if (other.CompareTag("PlaceableTile") || other.CompareTag("NonPlaceableTile") || other.CompareTag("DropZone") || other.CompareTag("TapeTile") || other.CompareTag("TableTile"))
        {
            PlayerManager.myPlayerLiftController.latestTile = other.gameObject;
            Renderer renderer = other.GetComponent<Renderer>();
            if (other.CompareTag("PlaceableTile"))
            {
                Highlight(renderer, standardTile);
            }
            else if (other.CompareTag("DropZone"))
            {
                Highlight(renderer, standardDropZone);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (PV.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;
        
        Renderer renderer = other.GetComponent<Renderer>();
        if (other.CompareTag("PlaceableTile")) renderer.material = standardTile;
        else if (other.CompareTag("DropZone")) renderer.material = standardDropZone;
    }

    private void Highlight(Renderer renderer, Material material)
    {
        Material matTemp = new Material(material);
        matTemp.SetColor("_Color", Color.blue);
        renderer.material = matTemp;
    }
}
