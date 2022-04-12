using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/*
    1. first player to get 10 kills wins (working)
    2. kill feed UI (working)
    3. show player names (working)
    4. respawn players in a set respawn area (working)
    BONUS: display player name that won (working) and return players to lobby (not working)
*/
public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP related stuff")]
    public float startHealth = 100f;
    private float health;
    public Image healthBar;
    public Animator animator;
    public float killCount = 0f;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (photonView.IsMine && killCount >= 10f) {
            photonView.RPC("PlayerWon", RpcTarget.All);
            GameManager.instance.LeaveRoom(); 
        }
    }

    public void Fire()
    {
        RaycastHit hit;

        Ray ray = camera.ViewportPointToRay(new Vector3 (0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200)) {
            Debug.Log(hit.collider.gameObject.name);
            
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);
            if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine) {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);

                //if player killed raycast target
                if(hit.collider.gameObject.GetComponent<Shooting>().health <= 0f) {
                    killCount++;
                    Debug.Log("Kill Count: " + killCount);

                    //as of now u can unlimited click a player to increase killcount. (for presentation sake only)
                }
            }
        }
    }

    [PunRPC]
    public void PlayerWon()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        respawnText.GetComponent<Text>().text = photonView.Owner.NickName + " has won the game!";

        Time.timeScale = 0;
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if(health <= 0f) {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

            GameManager.instance.KillFeedChange(info.Sender.NickName.ToString(), info.photonView.Owner.NickName.ToString());
        }
    }
    
    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);

        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine) {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while(respawnTime > 0) {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;

            respawnText.GetComponent<Text>().text = "You are dead. Respawning in: " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        int randomNumber = Random.Range(1, 5);

        Vector3 spawn1 = SpawnPoints.Instance.sp1;
        Vector3 spawn2 = SpawnPoints.Instance.sp2;
        Vector3 spawn3 = SpawnPoints.Instance.sp3;
        Vector3 spawn4 = SpawnPoints.Instance.sp4;

        if (randomNumber == 1) this.transform.position = spawn1;
        else if (randomNumber == 2) this.transform.position = spawn2;
        else if (randomNumber == 3) this.transform.position = spawn3;
        else if (randomNumber == 4) this.transform.position = spawn4;

        transform.GetComponent<PlayerMovementController>().enabled = true;
        
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }
}
