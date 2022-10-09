using System.Collections;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    public CanvasGroup infoTextCanvas;

    public void Awake()
    {
        infoTextCanvas = transform.GetComponent<CanvasGroup>();
    }
    
    public void ShowInfoText()
    {
        LeanTween.alphaCanvas(infoTextCanvas, 1f, 0.5f);
    }
    
    public void CloseInfoText()
    {
        if (gameObject.activeInHierarchy)
        {
            LeanTween.alphaCanvas(infoTextCanvas, 0f, 0.5f);
            StartCoroutine(DisableText());
        }
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }
}
