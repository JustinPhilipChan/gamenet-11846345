using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public GameObject[] finisherTextsUi;
    public Transform[] startingPositions;
    public Text timeText;

    public List<GameObject> lapTriggers = new List<GameObject>();
    public static RacingGameManager instance = null;

    void Awake()
    {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady) {
            object playerSelectionNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in finisherTextsUi) {
            go.SetActive(false);
        }
    }
}
