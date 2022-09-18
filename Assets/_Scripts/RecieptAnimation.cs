using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecieptAnimation : MonoBehaviour
{
    public AudioClip recieptPrint, recieptRip;
    private GameObject receipt;
    private AudioSource sfxSource;
    private CanvasGroup exitButton, resultText;
    void Start()
    {
        exitButton = transform.Find("QuitButton").GetComponent<CanvasGroup>();
        sfxSource = GameObject.Find("PlayerCanvas").GetComponent<AudioSource>();
        receipt = transform.Find("Receipt").gameObject;
        resultText = transform.Find("Banner2").GetComponent<CanvasGroup>();
        
        exitButton.alpha = 0f;
        resultText.alpha = 0f;
        
        Vector2 targetPos = receipt.transform.position;
        Vector2 initPos = new Vector2(targetPos.x, targetPos.y - Screen.height);
        receipt.transform.position = initPos;

        StartCoroutine(ResultAnimation(targetPos));
    }

    private IEnumerator ResultAnimation(Vector2 finalPos)
    {
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
        

        //Reveal quit button//
        yield return new WaitForSeconds(1f);
        LeanTween.alphaCanvas(exitButton, 1f, 0.5f);
    }
}
