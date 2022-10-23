using System;
using EZCameraShake;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    public Transform playerCam;
    private Vector3 zoomOutPos, plateCorrectedPos, centerPos;
    private StateManager stateManager;
    private CameraShaker camShaker;

    public void Start()
    {
        playerCam = transform.Find("Camera");
    }

    public void OnStartGame()
    {
        camShaker = GetComponent<CameraShaker>();
        stateManager = GameObject.Find("StateManager(Clone)").GetComponent<StateManager>();
        centerPos = stateManager.transform.position;

        //Player model should be facing the food, so it is rotated across y-axis
        Vector3 lookDir = (centerPos - transform.position) * 0.01f;
        Transform playerModel = transform.Find("Model").GetChild(0);
        playerModel.LookAt(transform.position +  lookDir);
        
        //Camera is looking ath the food more directly, so it should transform everything
        zoomOutPos = playerCam.position;
        playerCam.LookAt(centerPos);
    }
    
    public void UpdateCameraLook()
    {
        foreach (NetworkIdentity player in stateManager.activePlayers)
        {
            centerPos = GameObject.Find("StateManager(Clone)").transform.position;
        
            Vector3 lookDir = (centerPos - player.transform.position) * 0.01f;
            Transform playerModel = player.transform.Find("Model").GetChild(0);
            playerModel.LookAt(player.transform.position +  lookDir);
        }
        
        Vector3 platePos = GameObject.FindWithTag("Plate").transform.position; 
        plateCorrectedPos = new Vector3(platePos.x + 1f, platePos.y + 2.5f, platePos.z);
        playerCam.LookAt(plateCorrectedPos);
        
        
    }

    public void ZoomIn()
    {
        Vector3 direction = zoomOutPos - plateCorrectedPos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam.gameObject, zoomInPos, 1f).setEaseOutSine();
    }
    
    public void ZoomOut()
    {
        LeanTween.move(playerCam.gameObject, zoomOutPos, 1f).setEaseOutSine();
        UpdateCameraLook();
    }

    public void ShakeCamera(float length)
    {
        camShaker.ShakeOnce(5f, 15f, 0.1f, length);
    }
}
