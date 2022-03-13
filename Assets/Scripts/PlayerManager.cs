using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mirror;

public class PlayerManager : MonoBehaviour
{
    ///[SyncVar]
    public int health;

    //[HideInInspector]
    //[SyncVar]
    public bool isEncouraged, actionable, hasRecommended, orderVictim;

    [HideInInspector]
    public bool[] psnArray = new bool[4];

    [HideInInspector]
    //[SyncVar]
    public GameObject recommendedPiece;

    public void Start()
    {
        health = 2;
        actionable = false;
        isEncouraged = false;
        hasRecommended = false;
        orderVictim = false;
        recommendedPiece = null;
        
        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }
    }
}