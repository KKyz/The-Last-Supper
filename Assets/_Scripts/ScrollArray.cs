using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollArray : MonoBehaviour
{
    [SerializeField] private PlayerScrolls[] playerScrolls;
    
    public void SetValue(PlayerScrolls item, int index) 
    {
        playerScrolls[index] = item;
    }

    public PlayerScrolls GetValue (int index) 
    {
        return playerScrolls[index];
    }

    public int GetIndex(string scrollName)
    {
        for (int i = 0; i < playerScrolls.Length; i++)
        {
            if (playerScrolls[i].name == scrollName)
            {
                return i;
            }
        }

        return 0;
    }

    public void AddScrollAmount(string scrollName)
    {
        int index = GetIndex(scrollName); 
        //find index of item in list
        PlayerScrolls item = GetValue(index);
        item.amount += 1;
        SetValue(item, index);
    }

    public void RemoveScrollAmount(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetValue(index);
        item.amount -= 1;
        SetValue(item, index); 
    }

    public void ResetScrollAmount()
    {
        for (int i = 0; i < playerScrolls.Length; i++)
        {
            playerScrolls[i].amount = 0;
        }
    }

    public string GetDescription(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetValue(index); 
        return item.description;
    }
    
    public Sprite GetSprite(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetValue(index); 
        return item.scroll;
    }

    [System.Serializable]
    public class PlayerScrolls 
    {
        public int amount;
        public string name;
        public string description;
        public Sprite scroll;
    }
}

