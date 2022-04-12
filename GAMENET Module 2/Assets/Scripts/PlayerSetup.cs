﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;
    public GameObject playerUiPrefab;
    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;
    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;
    private Shooting shooting;

    [SerializeField] TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();
        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);

        shooting = this.GetComponent<Shooting>();

        animator.SetBool("isLocalPlayer", photonView.IsMine);
        
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;
        //long version of the code above
        /*if(photonView.IsMine)
        {
            this.animator.avatar = fpsAvatar;
        } else 
        {
            this.animator.avatar = nonFpsAvatar;
        }*/

        if (photonView.IsMine)
        {
            GameObject playerUi = Instantiate(playerUiPrefab);
            playerMovementController.fixedTouchField = playerUi.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;

            playerUi.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        } 
        else
        {
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }

        playerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
