using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour
{
    private Slider healthSlider;
    public Gradient gradient;
    public Image fill;

    public void Start()
    {
        healthSlider = transform.GetComponent<Slider>();
        fill.color = gradient.Evaluate(1f);
        SetHealth(2);
    }
    public void SetHealth(int health)
    {
        healthSlider.value = health;
        fill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }
}
