using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FoodPiece : NetworkBehaviour
{
    [SyncVar]
    public string type;
    
    [Server]
    public void SetType(int mode, float[] scrollProb)
    {
        if (mode == 0)
        {
            type = "Normal";
        }

        else if (mode == 1)
        {
            type = "Poison";
        }

        else
        {
            List<float> accScrollProb = new List<float>();
            //For every probability in scrollProb...
            //Start from beginning of list and add up every element until current index
            //Add new number into accScrollProb

            for (int i = 1; i <= scrollProb.Length; i++)
            {
                float accProb = 0;
                for (int j = 0; j < i; j++)
                {
                    accProb += scrollProb[j];
                }
                accScrollProb.Add(accProb);
            }

            // string scrollResult = "piecePos: ";
            // foreach (var item in accScrollProb)
            // {
            //     scrollResult += item.ToString() + ", ";
            // }
            // Debug.LogWarning(scrollResult);


            //Setting random.range as float as dividing integer by integer results in rounding to 0

            float prob = (float)Random.Range(0, 100)/100;
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

    [Command(requiresAuthority = false)]
    public void FakePsn()
    {
        //Add authority
        type = "FakePoison";
    }
}
