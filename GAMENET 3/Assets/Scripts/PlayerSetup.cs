using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        } 
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr")) {
            GetComponent<LapController>().enabled = false;
            GetComponent<CountdownManager>().enabled = false;
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<VehicleMovement>().isControlEnabled = photonView.IsMine;
            GetComponent<Shooting2Kinds>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        }
    }

    
}
