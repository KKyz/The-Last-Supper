using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFlagType : MonoBehaviour
{
    public List<Sprite> flags = new List<Sprite>();
    private Sprite currentFlag;

    void Start()
    {
        currentFlag = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public IEnumerator SetFlag(string type)
    {
        transform.name = "PlantedFlag";
        LeanTween.alpha(gameObject, 0, 0.7f);
        yield return new WaitForSeconds(0.71f);

        if (type == "Poison")
        {
            currentFlag = flags[0];
        }
        else if (type == "Health")
        {
            currentFlag = flags[1];
        }
        else if (type == "Skip")
        {
            currentFlag = flags[2];
        }
        else if (type == "Slap")
        {
            currentFlag = flags[3];
        }
        else if (type == "Smell")
        {
            currentFlag = flags[4];
        }
        else if (type == "Quake")
        {
            currentFlag = flags[5];
        }
        else if (type == "Order")
        {
            currentFlag = flags[6];
        }
        else
        {
            currentFlag = flags[7];
        }
        
        LeanTween.alpha(gameObject, 1, 0.7f);
        gameObject.GetComponent<SpriteRenderer>().sprite = currentFlag;
    }
}
