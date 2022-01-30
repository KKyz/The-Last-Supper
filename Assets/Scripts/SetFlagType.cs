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

    public void SetFlag(string type)
    {
        if (type == "Poison"){currentFlag = flags[0];}
        if (type == "Health"){currentFlag = flags[1];}
        if (type == "Skip"){currentFlag = flags[2];}
        if (type == "Slap"){currentFlag = flags[3];}
        if (type == "Order"){currentFlag = flags[4];}
        if (type == "Quake"){currentFlag = flags[5];}
    }

    void Update()
    {
        
    }
}
