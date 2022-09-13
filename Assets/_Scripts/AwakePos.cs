using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakePos : MonoBehaviour
{
    public Vector2 awakePos;
    
    public void Awake()
    {
        awakePos = transform.position;
    }

    public void UpdatePos(Vector2 newTransform)
    {
        awakePos = newTransform;
    }
}
