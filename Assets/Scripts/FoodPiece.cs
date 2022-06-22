using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FoodPiece : NetworkBehaviour
{
    [SyncVar]
    public string type;
    
    [HideInInspector]
    public bool isSelectable, isRecommended;

    [Server]
    public void SetType(bool setNorm, bool setPsn, float[] scrollProb)
    {
        if (setNorm)
        {
            type = "Normal";
        }

        else if (setPsn)
        {
            type = "Poison";
        }

        else
        {
            List<float> accScrollProb = new List<float>();
            //For every probability in scrollProb...
            //Start from beginning of list and add up every element until current index
            //Add new number into accScrollProb
            for (int i = 0; i < scrollProb.Length; i++)
            {
                float accProb = 0;
                for (int j = 0; j < i; j++)
                {
                    accProb += scrollProb[j];
                }
                accScrollProb.Add(accProb);
            }

            float prob = (Random.Range(0, 100))/100;
            if (prob <= accScrollProb[0])
            {
                type = "Order";
            }

            else if (prob <= accScrollProb[1])
            {
                type = "Quake";
            }

            else if (prob <= accScrollProb[2])
            {
                type = "Smell";
            }

            else if (prob <= accScrollProb[3])
            {
                type = "Health";
            }

            else if (prob <= accScrollProb[4])
            {
                type = "Slap";
            }

            else if (prob <= accScrollProb[5])
            {
                type = "Skip";
            }

            else if (prob <= accScrollProb[6])
            {
                type = "Encourage";
            }
        }     
    }

    [Server]
    public void FakePsn()
    {
        type = "FakePoison";
    }

    void Start()
    {
        isSelectable = false;
        isRecommended = false;
    }
}
