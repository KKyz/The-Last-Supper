using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour
{
    private Transform bar;
    public Color[] barGradient = new Color[4]; 
    private Image fill;
    public float[] fillLengths = new float[4];

    public void Start()
    {
        bar = transform.Find("Fill").transform;
        fill = bar.GetComponent<Image>();
        SetHealth(2);
    }
    public void SetHealth(int health)
    {
        fill.color = barGradient[health];
        
        fill.fillAmount = fillLengths[health];
    }
}
