using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
   public static SpawnPoints Instance { get; private set; }

   public Vector3 sp1 = new Vector3 (-6.5f, 0, 18.5f); 
   public Vector3 sp2 = new Vector3 (10.5f, 0, 8.5f);
   public Vector3 sp3 = new Vector3 (-18f, 0, -10f); 
   public Vector3 sp4 = new Vector3 (-12f, 0, -19f); 

   private void Awake()
   {
       if (Instance == null) {
           Instance = this;
           DontDestroyOnLoad(gameObject);
       } else {
           Destroy(gameObject); //prevents duplicates
       }
   }
}
