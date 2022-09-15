using System.Collections;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void FadeOut(float time)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        LeanTween.alphaCanvas(canvasGroup, 0, time);
        StartCoroutine(DisableFade(time));
    }

    public void FadeIn(float time)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, time);
    }
    
    private IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time + 2f);
        gameObject.SetActive(false);
    }
}
