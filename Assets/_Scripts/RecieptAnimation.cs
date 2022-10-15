using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RecieptAnimation : MonoBehaviour
{
    public AudioClip recieptPrint, recieptRip;
    private GameObject receipt;
    private AudioSource sfxSource;
    private Image background;
    private CanvasGroup exitButton, resultText, spectateButton;
    private PlayerFunctions playerFunctions;
    private Color bgColor;

    public void Spectate()
    {
        exitButton.GetComponent<Button>().interactable = false;
        spectateButton.GetComponent<Button>().interactable = false;

        exitButton.alpha = 0f;
        resultText.alpha = 0f;
        spectateButton.alpha = 0f;
        background.color = Color.clear;
        receipt.SetActive(false);
    }
    
    public void Start()
    {
        exitButton = transform.Find("QuitButton").GetComponent<CanvasGroup>();
        spectateButton = transform.Find("SpectateButton").GetComponent<CanvasGroup>();
        playerFunctions = transform.parent.GetComponent<PlayerFunctions>();
        sfxSource = playerFunctions.GetComponent<AudioSource>();
        receipt = transform.Find("Receipt").gameObject;
        resultText = transform.Find("Banner").GetComponent<CanvasGroup>();
        background = transform.Find("BG").GetComponent<Image>();

        bgColor = background.color;
        
        exitButton.GetComponent<Button>().interactable = false;
        spectateButton.GetComponent<Button>().interactable = false;
        
        exitButton.alpha = 0f;
        resultText.alpha = 0f;
        spectateButton.alpha = 0f;
        
        Vector2 targetPos = receipt.transform.position;
        Vector2 initPos = new Vector2(targetPos.x, targetPos.y - Screen.height);
        receipt.transform.position = initPos;

        StartCoroutine(ResultAnimation(targetPos));
        
    }

    private IEnumerator ResultAnimation(Vector2 finalPos)
    {
        yield return 0;
        
        //Reveal "you win/lose" text
        
        LeanTween.alphaCanvas(resultText, 1f, 0.5f);
        
        //"Printing" receipt animation
        yield return new WaitForSeconds(1.2f);
        LeanTween.moveY(receipt, finalPos.y - Screen.height/2f, 0.7f);
        sfxSource.PlayOneShot(recieptPrint);
        yield return new WaitForSeconds(1.5f);
        LeanTween.moveY(receipt, finalPos.y - Screen.height/2.5f, 0.3f);
        sfxSource.PlayOneShot(recieptPrint);
        yield return new WaitForSeconds(1.5f);
        LeanTween.moveY(receipt, finalPos.y, 0.7f).setEaseOutSine();
        sfxSource.PlayOneShot(recieptRip);
        

        //Reveal quit button
        if (playerFunctions.stateManager.activePlayers.Count == 0)
        {
            yield return new WaitForSeconds(1f);
            LeanTween.alphaCanvas(exitButton, 1f, 0.5f);
            exitButton.GetComponent<Button>().interactable = true; 
        }
        
        if (playerFunctions.stateManager.activePlayers.Count != 0)
        {
            yield return new WaitForSeconds(0.4f);
            LeanTween.alphaCanvas(spectateButton, 1f, 0.5f);
            spectateButton.GetComponent<Button>().interactable = true;
        }
    }
}
