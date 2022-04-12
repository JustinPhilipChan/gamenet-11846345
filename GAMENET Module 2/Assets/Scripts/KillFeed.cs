using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviour
{
    public static KillFeed KFinstance;

    //public Text killFeedText;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void ChangeKillFeedText(string killer, string killed) 
    {
        GameObject killFeedText = GameObject.Find("KillFeedText");
        killFeedText.GetComponent<Text>().text = killer + "   killed   " + killed;
        Debug.Log("Kill Feed Text has been changed!");
    }
}
