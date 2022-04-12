using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    GameObject player;
    public Text kfText;

    public static GameManager instance;

    void Awake()
    {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
        instance = this;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        int randomNumber = Random.Range(1, 5);

        Vector3 spawn1 = SpawnPoints.Instance.sp1;
        Vector3 spawn2 = SpawnPoints.Instance.sp2;
        Vector3 spawn3 = SpawnPoints.Instance.sp3;
        Vector3 spawn4 = SpawnPoints.Instance.sp4;

        if (PhotonNetwork.IsConnectedAndReady) {
            //alternate between 4 random spawn points
            if (randomNumber == 1) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawn1, Quaternion.identity);
            } else if (randomNumber == 2) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawn2, Quaternion.identity);
            } else if (randomNumber == 3) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawn3, Quaternion.identity);
            } else if (randomNumber == 4) {
                PhotonNetwork.Instantiate(playerPrefab.name, spawn4, Quaternion.identity);
            }
        }
    }

   public void KillFeedChange(string killer, string killed)
   {
       kfText.text = killer + "   killed   " + killed;
        Debug.Log("Kill Feed Text has been changed!");
   }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
