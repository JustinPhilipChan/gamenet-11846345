using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    private float startHealth = 100;
    public float health;
    [SerializeField] Image healthBar; 


    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
        healthBar.fillAmount = health / startHealth;
        if (health < 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // make sure player is yours before leaving the room
        if (photonView.IsMine) GameManager.instance.LeaveRoom();
    }
}
