﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpCheck : MonoBehaviour
{
    ProductController productController;
    Products product;
    PlayerController playerController;
    PhotonView PV;
    ProductController package;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        PV = playerController.GetComponent<PhotonView>();
    }

    void OntriggerEnter(Collider other)
    {
        if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "ProductController")
        {
            productController = other.GetComponent<ProductController>();
            productController.SetCanPickUp(true);
            //productController.setLatestPlayer(playerController.gameObject.transform);
        }
        else if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "Product")
        {
            product = other.GetComponent<Products>();
            product.SetCanPickUp(true);
            //product.SetLatestPlayer(playerController.gameObject.transform);
        }
        else if (other.gameObject != playerController.gameObject && other.gameObject.tag == "Package")
        {         
            package = other.GetComponent<ProductController>();
            package.SetCanPickUp(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "ProductController")
        {
            productController = other.GetComponent<ProductController>();
            productController.SetCanPickUp(false);
        }
        else if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "Product")
        {
            product = other.GetComponent<Products>();
            product.SetCanPickUp(false);
        }
        else if (other.gameObject != playerController.gameObject && other.gameObject.tag == "Package")
        {
            package = other.GetComponent<ProductController>();
            package.SetCanPickUp(false);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "ProductController")
        {
            productController = other.GetComponent<ProductController>();
            productController.SetCanPickUp(true);
            //productController.setLatestPlayer(playerController.gameObject.transform);
            
        }
        else if(other.gameObject != playerController.gameObject && PV.IsMine && other.gameObject.tag == "Product")
        {
            product = other.GetComponent<Products>();
            product.SetCanPickUp(true);
            //product.SetLatestPlayer(playerController.gameObject.transform);
        }
        else if (other.gameObject != playerController.gameObject && other.gameObject.tag == "Package")
        {
            package = other.GetComponent<ProductController>();          
            package.SetCanPickUp(true);          

        }
    }
}
