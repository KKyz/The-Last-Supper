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
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1f;
        StartCoroutine(DisableFade(time));
    }

    public void FadeIn(float time)
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, time);
    }
    
    private IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(0.6f);
        LeanTween.alphaCanvas(canvasGroup, 0, time);
        yield return new WaitForSeconds(time + 1f);
        gameObject.SetActive(false);
    }
}
