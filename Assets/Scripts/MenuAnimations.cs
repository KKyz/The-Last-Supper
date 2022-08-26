using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimations : MonoBehaviour
{
    public AnimEnum animationType;
    private Vector2 awakePos;
   
    public void Awake()
    {
        awakePos = transform.position;
    }

    public void EnterAnim()
    {
        if (animationType == AnimEnum.Up)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x, goalPos.y - 700);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Down)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x, goalPos.y + 700);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Left)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x - 700, goalPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Right)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x + 700, goalPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Fade)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            LeanTween.alphaCanvas(canvasGroup, 1, 0.8f);
        }
    }

    public void ExitAnim()
    {
        if (animationType == AnimEnum.Up)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x, startPos.y - 700);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Down)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x, startPos.y + 700);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Left)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x - 700, startPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Right)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x + 700, startPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Fade)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            LeanTween.alphaCanvas(canvasGroup, 0, 0.2f);
        }
    }
}

public enum AnimEnum
{
    Up, 
    Down, 
    Left,
    Right,
    Fade,
    None
};
