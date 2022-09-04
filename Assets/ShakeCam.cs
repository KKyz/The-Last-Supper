using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.NetworkRoom;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{
    private Transform playerCam;
    private Vector3 zoomOutPos;
    public float duration, amount, decrease;
    void Start()
    {
        playerCam = transform;
        zoomOutPos = playerCam.position;
    }
    
    void Update()
    {
        if (Input.GetKey("z"))
        {
            StartCoroutine(ShakeCamera(duration, amount, decrease));
        }
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
