using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecieptAnimation : MonoBehaviour
{
    private GameObject receipt;
    private CanvasGroup exitButton, resultText;
    void Start()
    {
        exitButton = transform.Find("QuitButton").GetComponent<CanvasGroup>();
        receipt = transform.Find("Receipt").gameObject;
        resultText = transform.Find("Banner2").GetComponent<CanvasGroup>();
        
        exitButton.alpha = 0f;
        resultText.alpha = 0f;
        
        Vector2 targetPos = receipt.transform.position;
        Vector2 initPos = new Vector2(targetPos.x, targetPos.y - 500f);
        receipt.transform.position = initPos;

        StartCoroutine(ResultAnimation(targetPos));
    }

    private IEnumerator ResultAnimation(Vector2 finalPos)
    {
        //Reveal "you win/lose" text
        
        LeanTween.alphaCanvas(resultText, 1f, 0.5f);
        
        //"Printing" receipt animation
        yield return new WaitForSeconds(1.2f);
        LeanTween.moveY(receipt, finalPos.y - 300f, 0.7f);
        yield return new WaitForSeconds(1.5f);
        LeanTween.moveY(receipt, finalPos.y - 200f, 0.3f);
        yield return new WaitForSeconds(1.5f);
        LeanTween.moveY(receipt, finalPos.y, 0.7f).setEaseOutSine();
        

        //Reveal quit button
        yield return new WaitForSeconds(1f);
        LeanTween.alphaCanvas(exitButton, 1f, 0.5f);
        



    }
}
