using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TweenTest : MonoBehaviour
{
    public List<GameObject> children = new List<GameObject>(); 
    void Start()
    {
        StartCoroutine(DespawnFlag(gameObject));
    }
    
    IEnumerator SpawnButton(List<GameObject> buttons)
    {
        foreach (GameObject button in buttons)
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 goalPos = button.transform.position;
            button.transform.position = new Vector3((goalPos.x - 10), goalPos.y, 0);
            button.SetActive(true);
            LeanTween.move(button, goalPos, 0.2f);
        }
    }
    
    IEnumerator DespawnButton(List<GameObject> buttons)
    {
        foreach (GameObject button in buttons)
        {
            Vector3 startPos = button.transform.position;
            Vector3 goalPos = new Vector3(startPos.x, (startPos.y - 100), 0);
            yield return new WaitForSeconds(0.2f);
            LeanTween.alpha(button, 0, 0.1f);
            LeanTween.move(button, goalPos, 0.2f);
        }

        foreach (GameObject button in buttons)
        {
            yield return new WaitForSeconds(0.3f);
            button.SetActive(false);
        }
    }

    IEnumerator SpawnFlag(GameObject flagObject)
    {
        yield return 0;
        Vector3 goalPos = flagObject.transform.position;
        Vector3 startPos = new Vector3(goalPos.x, (goalPos.y + 8f), goalPos.z);
        flagObject.transform.position = startPos;
        flagObject.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, 0);
        LeanTween.alpha(flagObject, 1, 0.9f);
        LeanTween.move(flagObject, goalPos, 0.6f);
    }
    
    IEnumerator DespawnFlag(GameObject flagObject)
    {
        yield return 0;
        Vector3 startPos = flagObject.transform.position;
        Vector3 goalPos = new Vector3(startPos.x, (startPos.y + 8f), startPos.z);
        LeanTween.alpha(flagObject, 0, 0.4f);
        LeanTween.move(flagObject, goalPos, 0.6f);
    }
}
