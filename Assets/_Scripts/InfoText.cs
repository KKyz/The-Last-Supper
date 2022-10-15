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
        LeanTween.alphaCanvas(infoTextCanvas, 2f, 0.5f);
    }
    
    public void CloseInfoText()
    {
        if (gameObject.activeInHierarchy)
        {
            LeanTween.alphaCanvas(infoTextCanvas, 0f, 0.5f);
        }
    }
}
