using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

/*
    CHALLENGES:
    Death Race Scene must be loaded when the player selected it as a game mode (CHECK)
    Parameters:
        3 maximum players (CHECK)
        Vehicles have HP (CHECK)

    Select vehicles that have weapons, or you may add some sort of turrets on our existing vehicles
    Create two varieties of weapons:
        Laser - raycast (CHECK)
        Projectile - rigidbody, spawned object (CHECK) 

    When a player dies, an event should be raised saying that he/she was eliminated (CHECK)
    Display the last man standing as a winner (PROBLEM: WinnerText wont show to other player even though ShowText() has the PunRPC attribute)
*/

public class Shooting2Kinds : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public float startHealth = 100f;
    public float currentHealth;
    bool isDead;

    [Header("LaserShooting")]
    public Camera playerCam;
    public Transform laserOrigin; //DOUBLES AS FIRE POINT
    public bool useLaser = false;
    public LineRenderer laserLine;
    bool canLaser = false;

    [Header("ProjectileShooting")]
    public GameObject projectilePrefab;
    public float shootForce, timeBetweenShooting, reloadTime;
    public int bulletDamage, magazineSize;
    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;
    [Header("Colliders")]
    public BoxCollider triggerBox, colliderBox;
    public enum RaiseEventsCode
    {
        WhoDiedFirst = 0
    }
    
    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent) 
    {
        if(photonEvent.Code == (byte) RaiseEventsCode.WhoDiedFirst) {
            //event has been sent for whodiedfirst
            //retrieve the data
            object[] data = (object[]) photonEvent.CustomData;
            string nickNameOfPlayerWhoDied = (string) data[0];
            finishOrder = (int) data[1];
            int viewId = (int) data[2];

            Debug.Log("EVENT WAS RAISED: " + nickNameOfPlayerWhoDied + " " + finishOrder);

            GameObject orderUIText = DeathRaceManager.DRinstance.PlacesUI[finishOrder];
            orderUIText.SetActive(true);

            if (viewId == photonView.ViewID) {
                orderUIText.GetComponent<Text>().text = (finishOrder + 1) + " " + nickNameOfPlayerWhoDied + " (YOU)";
            } else {
                orderUIText.GetComponent<Text>().text = (finishOrder + 1) + " " + nickNameOfPlayerWhoDied;
            }
        }
    }

    void Awake()
    {
        currentHealth = startHealth;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        canLaser = true;
    }

    void Start()
    {
        finishOrder = DeathRaceManager.DRinstance.playersLeft;  //sets it at either 3 or 2 or 1 (based on max players)
    }

    void Update()
    {
        if (useLaser) {
            if(Input.GetMouseButton(0) && canLaser) {
                laserLine.enabled = true;

                Ray ray = playerCam.ViewportPointToRay(new Vector3 (0.5f, 0.45f));
                RaycastHit hit;

                laserLine.SetPosition(0, laserOrigin.position);
            
                if (Physics.Raycast(ray, out hit)) {
                    if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine && !hit.collider.gameObject.GetComponent<Shooting2Kinds>().isDead) {
                        //Debug.Log(hit.collider.name);
                        laserLine.SetPosition(1, hit.point);
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 5);
                        Debug.Log(hit.collider.gameObject.GetComponent<Shooting2Kinds>().currentHealth);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0)) laserLine.enabled = false;
        } 
        else { //NO LASER
            shooting = Input.GetKey(KeyCode.Mouse0); //holding left = shooting true

            //reloading
            if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) 
                Reload();
            //automatic reload when no bullets left
            if(readyToShoot && shooting && !reloading && bulletsLeft <= 0)
                Reload();

            if(readyToShoot && shooting && !reloading && bulletsLeft > 0) {
                bulletsShot = 0;

                //actual shooting
                readyToShoot = false;
                
                //slightly below center so the camera or cars wont obstruct the view
                //works kind of like a bullet drop thing
                Ray ray = playerCam.ViewportPointToRay(new Vector3 (0.5f, 0.45f)); 
                RaycastHit hit;
                Vector3 targetPoint;
                if(Physics.Raycast(ray, out hit)) {
                    targetPoint = hit.point;
                    if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine && !hit.collider.gameObject.GetComponent<Shooting2Kinds>().isDead) {
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, bulletDamage);
                        Debug.Log(hit.collider.gameObject.GetComponent<Shooting2Kinds>().currentHealth);
                    }
                }
                else
                    targetPoint = ray.GetPoint(100); //just a random point away from the player

                //calculate direction
                Vector3 dir = targetPoint - laserOrigin.position;

                //Spawn projectile
                GameObject currentBullet = Instantiate(projectilePrefab, laserOrigin.position, Quaternion.identity);
                //rotate bullet to shoot direction
                currentBullet.transform.forward = dir.normalized;

                currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);

                bulletsLeft--;
                bulletsShot++;

                Invoke("ResetShot", timeBetweenShooting);
            }
        }

        if(finishOrder <= 1) 
            photonView.RPC("ShowWinner", RpcTarget.All);
    }

    [PunRPC]
    public void TakeDamage(int amount, PhotonMessageInfo info)
    {
        currentHealth -= amount;

        if(currentHealth <= 0f) {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void ShowWinner()
    {
        //Display winner text when last player standing is u and u aren't dead
        if (photonView.IsMine && !isDead) {
            DeathRaceManager.DRinstance.winnerText.enabled = true;
            DeathRaceManager.DRinstance.winnerText.text = "WINNER: " + photonView.Owner.NickName;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
    
    private void Die()
    {
        readyToShoot = false;
        canLaser = false;
        isDead = true;

        if(photonView.IsMine) {
            //GetComponent<Shooting2Kinds>().enabled = false;
            GetComponent<VehicleMovement>().enabled = false;
            //GetComponent<PlayerSetup>().camera.transform.parent = null;
            
            //ideally stops player from raising multiple events by disabling the collision/trigger
            triggerBox.enabled = false;
            colliderBox.enabled = false;
            finishOrder--;

            string nickName = photonView.Owner.NickName;
            int viewId = photonView.ViewID;

            object[] data = new object[] { nickName, finishOrder, viewId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };
            PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoDiedFirst, data,  raiseEventOptions, sendOptions);

            //DEDUCT 1 PLAYER FROM PLAYERSLEFT AFTER RAISING EVENT
            DeathRaceManager.DRinstance.DeductOnePlayer();
        }
    }

}
