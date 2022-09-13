using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCenter : MonoBehaviour
{
    private Transform center;
    private Transform playerCam;
 
    public void Start()
    {
        Quaternion initialRotation = Quaternion.Euler(new Vector3(this.transform.localRotation.eulerAngles.x,this.transform.localRotation.eulerAngles.y,this.transform.localRotation.eulerAngles.z));
        playerCam = transform.GetChild(0);
        center = GameObject.Find("GameManager").transform;
        transform.LookAt(center);
        transform.localRotation = Quaternion.Euler(new Vector3(initialRotation.x, this.transform.localRotation.y, initialRotation.z));
        playerCam.LookAt(new Vector3(center.transform.position.x + 1f, center.transform.position.y, center.transform.position.z));
    }
}
