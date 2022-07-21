using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    
    public int health, scrollCount, courseCount, pieceCount, timer, piecesEaten;

    public bool actionable;

    [SyncVar] 
    public bool isEncouraged, hasRecommended, orderVictim;

    [SyncVar(hook=nameof(SyncPsn))] 
    public bool psn0, psn1, psn2, psn3;

    public bool[] psnArray = new bool[4];
    
    [SyncVar]
    public GameObject recommendedPiece;
    
    private GameObject playerCam;
    private StateManager stateManager;

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

        playerCam = transform.Find("Camera").gameObject;
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        stateManager.spawnedPlayers.Add(GetComponent<NetworkIdentity>().netId, this);
        
        if (!isLocalPlayer)
        {
            playerCam.SetActive(false);
        }
    }
    
    
    public void SyncPsn(bool oldValue, bool newValue)
    {
        /*Shouldn't this function run on both server and specific client? (Is TargetRpc required?)*/
        psnArray[0] = psn0;
        psnArray[1] = psn1;
        psnArray[2] = psn2;
        psnArray[3] = psn3;
    }

    public void Eject()
    {
        GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>().Die();
    }
}