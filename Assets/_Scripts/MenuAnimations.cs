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
            Vector2 startPos = new Vector2(goalPos.x, goalPos.y - Screen.height);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Down)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x, goalPos.y + Screen.height);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Left)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x - Screen.width, goalPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Right)
        {
            Vector2 goalPos = awakePos;
            Vector2 startPos = new Vector2(goalPos.x + Screen.width, goalPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Fade)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            LeanTween.alphaCanvas(canvasGroup, 1, 0.8f);
        }
    }

    public void ExitAnim()
    {
        if (animationType == AnimEnum.Up)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x, startPos.y - Screen.height);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Down)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x, startPos.y + Screen.height);
            transform.position = startPos;
            LeanTween.moveY(gameObject, goalPos.y, 0.15f);
        }
        
        if (animationType == AnimEnum.Left)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x - Screen.width, startPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Right)
        {
            Vector2 startPos = awakePos;
            Vector2 goalPos = new Vector2(startPos.x + Screen.width, startPos.y);
            transform.position = startPos;
            LeanTween.moveX(gameObject, goalPos.x, 0.15f);
        }
        
        if (animationType == AnimEnum.Fade)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
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
