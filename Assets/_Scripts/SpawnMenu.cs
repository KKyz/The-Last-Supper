using System;
using UnityEngine;

public class SpawnMenu : MonoBehaviour
{
    private PlayerFunctions playerFunctions;

    public void Start()
    {
        playerFunctions = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
    }

    public void SlideInMenu()
    {
        CanvasGroup menuCanvas = transform.GetComponent<CanvasGroup>();
        Vector2 goalPos = transform.position;
        Vector2 startPos = new Vector3(goalPos.x, (goalPos.y + Screen.height));
        transform.position = startPos;
        LeanTween.moveY(gameObject, goalPos.y, 1f).setEaseInOutBack();
        LeanTween.alphaCanvas(menuCanvas, 1, 0.85f);
    }
    
    public void SlideOutMenu()
    {
        CanvasGroup menuCanvas = transform.GetComponent<CanvasGroup>();
        Vector2 startPos =  transform.position;
        Vector2 goalPos = new Vector3(startPos.x, (startPos.y + Screen.height));
        LeanTween.moveY(gameObject, goalPos.y, 1f).setEaseInOutBack().setLoopPingPong(1);
        LeanTween.alphaCanvas(menuCanvas, 0, 0.85f);
        Destroy(gameObject, 0.6f);
        playerFunctions.openPopup = null;
    }

    public void CanContinue()
    {
        playerFunctions.player.CmdSwitchContinueState(true);
    }
}
