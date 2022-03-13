using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPiece : MonoBehaviour
{
    [HideInInspector]
    public string type;
    
    [HideInInspector]
    public bool isSelectable, isRecommended;

    [HideInInspector]
    public GameObject recSign;
    
    private GameObject newRec;
    private int typeSelect;

    void Start()
    {
        isSelectable = false;
        isRecommended = false;

        typeSelect = Random.Range(0, 100);
        {
            if (typeSelect <= 2)
            {
                type = "Poison";
            }

            else if (3 <= typeSelect && typeSelect <= 10)
            {
                type = "Order";
            }

            else if (10 <= typeSelect && typeSelect <= 17)
            {
                type = "Quake";
            }

            else if (18 <= typeSelect && typeSelect <= 23)
            {
                type = "Smell";
            }

            else if (24 <= typeSelect && typeSelect <= 28)
            {
                type = "Health";
            }

            else if (29 <= typeSelect && typeSelect <= 35)
            {
                type = "Slap";
            }

            else if (36 <= typeSelect && typeSelect <= 42)
            {
                type = "Skip";
            }

            else if (43 <= typeSelect && typeSelect <= 52)
            {
                type = "Encourage";
            }

            else if (53 <= typeSelect)
            {
                type = "Normal";
            }        

        } 
    }

    void Update()
    {
        
    }

    void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (!isRecommended)
            {
                newRec = Instantiate(recSign, new Vector3(transform.position.x + 0.75f, transform.position.y + 1.25f, transform.position.z), Quaternion.identity);
                newRec.transform.SetParent(transform);
                isRecommended = true;
            }
            else
            {
                Destroy(newRec);
                isRecommended = false;
            }
        }
    }
}
