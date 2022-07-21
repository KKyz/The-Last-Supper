using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TweenTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DespawnButtons(gameObject));
    }
    
    public IEnumerator SpawnButtons(GameObject buttons)
    {
        buttons.SetActive(true);
        
        if (buttons.transform.childCount > 2)
        {
            foreach (Transform button in buttons.transform)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    yield return new WaitForSeconds(0.2f);
                    Vector3 goalPos = button.position;
                    CanvasGroup buttonCG = button.GetComponent<CanvasGroup>();
                    Vector3 startPos = new Vector3(goalPos.x, (goalPos.y - 300), 0);
                    buttonCG.alpha = 0f;
                    button.position = startPos;
                    LeanTween.alphaCanvas(buttonCG, 1, 0.6f);
                    LeanTween.move(button.gameObject, goalPos, 0.4f);
                }
            }
        }
        
        // Spawn individual button (e.g. cancel button)
        else
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 goalPos = buttons.transform.position;
            Vector3 startPos = new Vector3(goalPos.x, (goalPos.y - 300), 0);
            buttons.transform.position = startPos;
            LeanTween.alpha(buttons.gameObject, 1, 0.4f);
            LeanTween.move(buttons, goalPos, 0.3f);
        }
    }

    IEnumerator DespawnButtons(GameObject buttons)
    {
        if (buttons.transform.childCount > 1)
        {
            // Despawn Children of buttons (e.g. action buttons, scroll buttons)
            foreach (Transform button in buttons.transform)
            {
                if (button.gameObject.activeInHierarchy)
                {
                    Vector3 startPos = button.position;
                    Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 100), 0);
                    yield return new WaitForSeconds(0.2f);
                    CanvasGroup buttonCG = button.GetComponent<CanvasGroup>();
                    LeanTween.alphaCanvas(buttonCG, 0, 0.1f);
                    LeanTween.move(button.gameObject, goalPos, 0.2f);
                }
            }

            foreach (Transform button in buttons.transform)
            {
                yield return new WaitForSeconds(0.3f);
                button.gameObject.SetActive(false);
            }
        }

        // Despawn individual button (e.g. cancel button)
        else
        {
            if (buttons.gameObject.activeInHierarchy)
            {
                Vector3 startPos = buttons.transform.position;
                Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 100), 0);
                yield return new WaitForSeconds(0.2f);
                CanvasGroup buttonCG = buttons.GetComponent<CanvasGroup>();
                LeanTween.alphaCanvas(buttonCG, 0, 0.1f);
                LeanTween.move(buttons.gameObject, goalPos, 0.2f);
            }

            yield return new WaitForSeconds(0.3f);
            buttons.gameObject.SetActive(false);
        }
    }
}
