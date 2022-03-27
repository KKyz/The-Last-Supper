using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [HideInInspector]
    public int health, scrollCount, courseCount, pieceCount, timer, piecesEaten;

    [HideInInspector]
    public bool isEncouraged, actionable, hasRecommended, orderVictim;

    //HideInInspector]
    public bool[] psnArray = new bool[4];

    [HideInInspector]
    public GameObject recommendedPiece;
    
    private GameObject playerCam;

    public void Start()
    {
        health = 2;
        scrollCount = 0;
        courseCount = 1;
        pieceCount = 0;
        piecesEaten = 0;
        timer = 0;
        actionable = false;
        isEncouraged = false;
        hasRecommended = false;
        orderVictim = false;
        recommendedPiece = null;
        
        for (int i = 0; i < 4; i++)
        {
            psnArray[i] = false;
        }

        playerCam = transform.GetChild(0).gameObject;
        if(!isLocalPlayer) {playerCam.SetActive(false);}
    }
}