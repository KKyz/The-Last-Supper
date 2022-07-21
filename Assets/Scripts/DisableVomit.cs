using System.Collections;
using UnityEngine;

public class DisableVomit : MonoBehaviour
{
    private CanvasGroup vomitCanvas;
    
    void Start()
    {
        vomitCanvas = GetComponent<CanvasGroup>();
        StartCoroutine(DisableSplash());
    }

    private IEnumerator DisableSplash()
    {
        LeanTween.alphaCanvas(vomitCanvas, 0f, 0.8f);
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }
}
