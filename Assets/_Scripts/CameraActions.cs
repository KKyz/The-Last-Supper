using System;
using EZCameraShake;
using UnityEditor.Search.Providers;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    public Transform playerCam;
    private Vector3 zoomOutPos, plateCorrectedPos, centerPos;
    private CameraShaker camShaker;
    public float value = 0.01f;

    public void OnStartGame()
    {
        camShaker = GetComponent<CameraShaker>();
        centerPos = GameObject.Find("StateManager(Clone)").transform.position;
        Debug.LogWarning(centerPos);
        
        //Player model should be facing the food, so it is rotated across y-axis
        Vector3 lookDir = (centerPos - transform.position) * 0.01f;
        Transform playerModel = transform.Find("Model").GetChild(0);
        playerModel.LookAt(transform.position +  lookDir);
        
        //Camera is looking ath the food more directly, so it should transform everything
        playerCam = transform.Find("Camera");
        zoomOutPos = playerCam.position;
        playerCam.LookAt(centerPos);
    }

    public void UpdateCameraLook()
    {
        Vector3 platePos = GameObject.FindWithTag("Plate").transform.position; 
        plateCorrectedPos = new Vector3(platePos.x + 1f, platePos.y + 2.5f, platePos.z);
        Quaternion rotation = Quaternion.LookRotation (plateCorrectedPos - transform.position);
        playerCam.rotation = Quaternion.Slerp (playerCam.rotation, rotation, Time.deltaTime);
        
        Vector3 lookDir = (centerPos - transform.position) * 0.01f;
        Transform playerModel = transform.Find("Model").GetChild(0);
        playerModel.LookAt(transform.position +  lookDir);
    }

    public void ZoomIn()
    {
        Vector3 direction = zoomOutPos - plateCorrectedPos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam.gameObject, zoomInPos, 1f).setEaseOutSine();
        //Debug.LogWarning(playerCam.position + ", " + plateCorrectedPos + ", " + direction);
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
