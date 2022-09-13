using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        var color = text.color;
        var fadeoutcolor = color;
        fadeoutcolor.a = 0;
        LeanTween.value(gameObject, updateValueExampleCallback, fadeoutcolor, color, 1f);
    }


    void updateValueExampleCallback(Color val)
    {
        text.color = val;
    }

    public IEnumerator CycleTextAlpha()
    {
        yield return 0;
    }
    
    public void CloseInfoText()
    {
        gameObject.SetActive(false);
    }
    
    void Update()
    {

    }
}
