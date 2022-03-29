using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TweenTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SpawnButtons(gameObject));
    }
    
    IEnumerator SpawnButtons(GameObject buttons)
    {
        buttons.SetActive(true);
        
        if (buttons.transform.childCount > 1)
        {
            // Spawn Children of buttons (e.g. action buttons, scroll buttons)
            foreach (Transform button in buttons.transform)
            {
                if (button.name != "SlapButton" || button.name != "RecommendButton")
                {
                    if (!button.gameObject.activeInHierarchy)
                    {
                        Vector3 goalPos = button.position;
                        yield return new WaitForSeconds(0.2f);
                        button.gameObject.SetActive(true);
                        button.position = new Vector3((goalPos.x - 100), goalPos.y, 0);
                        LeanTween.move(button.gameObject, goalPos, 0.2f);
                    }
                }
            }
        }
        
        // Spawn individual button (e.g. cancel button)
        else
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 goalPos = buttons.transform.position;
            buttons.transform.position = new Vector3((goalPos.x - 10), goalPos.y, 0);
            buttons.gameObject.SetActive(true);
            LeanTween.move(buttons, goalPos, 0.2f);
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
                    //LeanTween.alpha(button.gameObject, 0, 0.1f);
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
                LeanTween.alpha(buttons.gameObject, 0, 0.5f);
                LeanTween.move(buttons.gameObject, goalPos, 0.2f);
            }

            yield return new WaitForSeconds(0.3f);
            buttons.gameObject.SetActive(false);
        }
    }
}
