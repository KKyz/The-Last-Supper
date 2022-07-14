using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour
{
    private Transform bar;
    public Vector2[] barLeftBottomStretches = new Vector2[4];
    public Vector2[] barRightTopStretches = new Vector2[4];
    public Color[] barGradient = new Color[4]; 
    private Image fill;
    private RectTransform stretch;

    public void Start()
    {
        bar = transform.Find("Fill").transform;
        fill = bar.GetComponent<Image>();
        stretch = bar.GetComponent<RectTransform>();
        SetHealth(2);
    }
    public void SetHealth(int health)
    {
        stretch.offsetMin = barLeftBottomStretches[health];
        stretch.offsetMax = barRightTopStretches[health];
        fill.color = barGradient[health];
    }
}
