using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public int health, scrollCount, courseCount, pieceCount, timer, piecesEaten;

    public bool actionable;

    [SyncVar] public bool isEncouraged, hasRecommended, orderVictim;

    [SyncVar] 
    public bool psn0, psn1, psn2, psn3;

    public bool[] psnArray = new bool[4];

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

    [ClientRpc]
    public void SyncPsn()
    {
        psnArray[0] = psn0;
        psnArray[1] = psn1;
        psnArray[2] = psn2;
        psnArray[3] = psn3;
    }
}