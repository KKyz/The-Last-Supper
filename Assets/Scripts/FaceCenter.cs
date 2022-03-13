using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCenter : MonoBehaviour
{
    private Transform center;
 
    public void Start()
    {
        center = GameObject.Find("GameManager").transform;
        transform.LookAt(new Vector3(center.transform.position.x + 1f, center.transform.position.y, center.transform.position.z));
    }
}
