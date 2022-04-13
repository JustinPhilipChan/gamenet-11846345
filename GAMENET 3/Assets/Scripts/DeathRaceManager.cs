using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DeathRaceManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] PlacesUI;
    public Text winnerText;

    public static DeathRaceManager DRinstance = null;

    public int playersLeft;

    void Awake()
    {
        if (DRinstance != null) {
            Destroy(this.gameObject);
        } else {
            DRinstance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        playersLeft = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("Players left: " + playersLeft);
        
        if(PhotonNetwork.IsConnectedAndReady) {
            object playerSelectionNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in PlacesUI) {
            go.SetActive(false);
        }
        winnerText.enabled = false;
    }

    public void DeductOnePlayer()
    {
        playersLeft--;
    }
}
