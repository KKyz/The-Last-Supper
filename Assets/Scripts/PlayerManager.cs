using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< Updated upstream
//using Mirror;
=======
using Mirror;
>>>>>>> Stashed changes

public class PlayerManager : NetworkBehaviour
{
<<<<<<< Updated upstream
    ///[SyncVar]
    public int health;

    //[HideInInspector]
    //[SyncVar]
=======
    public int health;

    //[HideInInspector]
>>>>>>> Stashed changes
    public bool isEncouraged, actionable, hasRecommended, orderVictim;

    [HideInInspector]
    public bool[] psnArray = new bool[4];

    [HideInInspector]
<<<<<<< Updated upstream
    //[SyncVar]
    public GameObject recommendedPiece;

    public void Start()
=======
    public GameObject recommendedPiece;

    public void OnClientStart()
>>>>>>> Stashed changes
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