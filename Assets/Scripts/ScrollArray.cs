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

    public void AddScrollAmount(int addedAmount, int index) 
    {
        PlayerScrolls item = GetValue(index);
        item.amount += addedAmount;
        SetValue(item, index);
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

