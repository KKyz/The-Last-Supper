using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodPiece : NetworkBehaviour
{
    [SyncVar]
    public string type;

    [Server]
    public void SetType(int mode, float scrollProb, string[] activeScrolls)
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
            List<float> scrollProbs = new List<float>();
            //For every probability in scrollProb...
            //Start from beginning of list and add up every element until current index
            //Add new number into accScrollProb

            float accScrollProb = 0f;
            for (int i = 1; i <= activeScrolls.Length; i++)
            {
                accScrollProb += scrollProb;
                scrollProbs.Add(accScrollProb);
            }
            
            type = RandomScroll(activeScrolls, scrollProbs);
        }
    }

    private string RandomScroll(string[] activeScrolls, List<float> scrollProbs)
    {
        float random = 1f;
        
        if (activeScrolls.Length > 1)
        {
            random = (float)Random.Range(0, 100)/100;
        }
            
        foreach (float prob in scrollProbs)
        {
            if (random <= prob)
            {
                return activeScrolls[scrollProbs.IndexOf(prob)];
            }
        }

        return RandomScroll(activeScrolls, scrollProbs);

    }

    [Command(requiresAuthority = false)]
    public void FakePsn()
    {
        //Add authority
        type = "FakePoison";
    }
}
