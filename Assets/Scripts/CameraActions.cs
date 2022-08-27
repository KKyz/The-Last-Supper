using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    public float magnitude;
    
    private Transform center, playerCam;
    private Vector3 zoomOutPos, centerPos;

    public void Start()
    {
        //Player model should be facing the food, so it is rotated across y-axis
        Quaternion initialRotation = Quaternion.identity;;
        center = GameObject.Find("StateManager").transform;
        transform.localRotation = Quaternion.Euler(new Vector3(initialRotation.x, transform.localRotation.y, initialRotation.z));
        
        //Camera is looking ath the food more directly, so it should transform everything
        playerCam = transform.Find("Camera");
        centerPos = center.position;
        zoomOutPos = playerCam.position;
        playerCam.LookAt(centerPos);
    }
    
    public void UpdateCameraLook()
    {
        Vector3 platePos = GameObject.FindWithTag("Plate").transform.position;
        centerPos = new Vector3(platePos.x + 1f, platePos.y + 2.5f, platePos.z);
        playerCam.LookAt(centerPos); 
    }

    public void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            UpdateCameraLook();
        }
    }

    public void ZoomIn()
    {
        Vector3 direction = zoomOutPos - centerPos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam.gameObject, zoomInPos, 1f).setEaseOutSine();
    }
    
    public void ZoomOut()
    {
        LeanTween.move(playerCam.gameObject, zoomOutPos, 1f).setEaseOutSine();
    }
}
