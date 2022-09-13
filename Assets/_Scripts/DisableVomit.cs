using System.Collections;
using UnityEngine;

public class DisableVomit : MonoBehaviour
{
    void Start()
    {
        CanvasGroup vomitCanvas = GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(vomitCanvas, 0f, 0.8f);
        Destroy(gameObject, 0.9f);
    }
}
