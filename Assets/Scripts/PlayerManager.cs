using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    
    public int scrollCount, courseCount, pieceCount, timer, piecesEaten;

    [SyncVar]
    public int health;

    public bool actionable;

    [SyncVar] 
    public bool isEncouraged, hasRecommended, orderVictim;

    [SyncVar(hook=nameof(SyncPsn))] 
    public bool psn0, psn1, psn2, psn3;

    public bool[] psnArray = new bool[4];
    
    [SyncVar(hook=nameof(SyncRecommended))]
    public GameObject recommendedPiece;

    public GameObject currentRecommend;
    
    private GameObject playerCam;
    private PlayerFunctions playerCanvas;
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
        playerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
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
    
    [Command(requiresAuthority = false)]
    public void CmdCreateRecommend(GameObject piece)
    {
        if (piece == recommendedPiece)
        {
            recommendedPiece = null;
        }
        else
        {
            recommendedPiece = piece;
        }
    }

    public void SyncRecommended(GameObject oldValue, GameObject newValue)
    {
        if (oldValue == null)
        {
            Debug.Log("PlayerManager: " + netId);
            
            //If the piece doesn't have any flags already, create one
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommend = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x + 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
            currentRecommend.transform.SetParent(recommendedPiece.transform);
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommend));
        }
        
        else if (newValue == null)
        {
            //If destroying pre-existing recommend
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommend));
            currentRecommend = null;
        }
        
        else
        {
            //Replace flag with a new one at a different piece
            StartCoroutine(playerCanvas.DespawnBillboard(currentRecommend));
            Vector3 pTrans = recommendedPiece.transform.position;
            currentRecommend = Instantiate(playerCanvas.recommendFlag, new Vector3(pTrans.x + 0.75f, pTrans.y + 1f, pTrans.z), Quaternion.identity);
            currentRecommend.transform.SetParent(recommendedPiece.transform);
            StartCoroutine(playerCanvas.SpawnBillboard(currentRecommend));
        }
        hasRecommended = true;
    }

    public void Eject()
    {
        playerCanvas.Die();
    }
}