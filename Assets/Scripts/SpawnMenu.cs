using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMenu : MonoBehaviour
{
    public void SlideInMenu()
    {
        Vector3 goalPos = transform.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 650f), goalPos.z);
        transform.position = startPos;
        LeanTween.move(gameObject, goalPos, 0.3f);
    }
    
    public void SlideOutMenu()
    {
        Vector3 startPos =  transform.position;
        Vector3 goalPos = new Vector3(startPos.x, (startPos.y + 650f), startPos.z);
        LeanTween.move(gameObject, goalPos, 0.3f);
        Destroy(gameObject, 1f);
    }
}
