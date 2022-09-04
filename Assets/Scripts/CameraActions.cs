using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    private Transform playerCam;
    private Vector3 zoomOutPos, plateCorrectedPos, centerPos;

    public void Start()
    {
        //Player model should be facing the food, so it is rotated across y-axis
        centerPos = GameObject.Find("StateManager").transform.position;
        Vector3 lookTowards = new Vector3(0, centerPos.y, 0);
        //transform.LookAt(lookTowards);

        //Camera is looking ath the food more directly, so it should transform everything
        playerCam = transform.Find("Camera");
        zoomOutPos = playerCam.position;
        playerCam.LookAt(centerPos);
    }
    
    public void UpdateCameraLook()
    {
        Vector3 platePos = GameObject.FindWithTag("Plate").transform.position; 
        plateCorrectedPos = new Vector3(platePos.x + 1f, platePos.y + 2.5f, platePos.z);
        playerCam.LookAt(plateCorrectedPos); 
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
        Vector3 direction = zoomOutPos - plateCorrectedPos;
        Vector3 zoomInPos = zoomOutPos - (direction * 0.4f);
        LeanTween.move(playerCam.gameObject, zoomInPos, 1f).setEaseOutSine();
    }
    
    public void ZoomOut()
    {
        LeanTween.move(playerCam.gameObject, zoomOutPos, 1f).setEaseOutSine();
    }

    public IEnumerator ShakeCamera(float shakeDuration, float shakeAmount, float decreaseFactor)
    {
        if (shakeDuration > 0)
        {
            playerCam.localPosition = zoomOutPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
			
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            playerCam.localPosition = zoomOutPos;
        }

        yield return null;
    }
}
