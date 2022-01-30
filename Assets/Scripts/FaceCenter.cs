using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCenter : MonoBehaviour
{
    private Transform center, seats, myPlayer;
 
    void Start()
    {
        myPlayer = transform.parent;
        seats = myPlayer.parent;
        center = seats.parent;
        transform.LookAt(new Vector3(center.transform.position.x, center.transform.position.y, center.transform.position.z));
    }
}
