using System;
using Mirror;
using UnityEngine;

public class ScrollArray : NetworkBehaviour
{
    public readonly SyncList<PlayerScrolls> playerScrolls = new();
    public readonly SyncList<int> scrollAmounts = new();
    public PlayerScrolls[] allScrolls;
    public Sprite[] scrollSprites;

    public PlayerScrolls GetScroll(int index) 
    {
        if (index < playerScrolls.Count)
        {
            return playerScrolls[index];
        }

        return null;
    }

    private int GetIndex(string scrollName)
    {
        for (int i = 0; i < playerScrolls.Count; i++)
        {
            if (playerScrolls[i].name == scrollName)
            {
                return i;
            }
        }

        return 0;
    }

    public int NumberOfScrolls()
    {
        int counter = 0;
        int index = 0;
        foreach (PlayerScrolls scroll in playerScrolls)
        {
            if (scrollAmounts[index] > 0)
            {
                counter += 1;
            }

            index++;
        }

        return counter;
    }

    public int GetScrollAmount(int index)
    {
        return scrollAmounts[index];
    }
    
    public int GetScrollAmount(string scrollName)
    {
        int index = GetIndex(name);
        return scrollAmounts[index];
    }

    [Command(requiresAuthority = false)]
    public void CmdAddScrollAmount(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetScroll(index);
        scrollAmounts[index] += 1;
    }

    [Command(requiresAuthority = false)]
    public void CmdRemoveScrollAmount(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetScroll(index);
        scrollAmounts[index] -= 1;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdResetScrollAmount()
    {
        for (int index = 0; index < playerScrolls.Count; index++)
        {
            scrollAmounts[index] = 0;
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdGiveAllScrolls()
    {
        for (int index = 0; index < playerScrolls.Count; index++)
        {
            scrollAmounts[index] = 1;
        }
    }

    public string GetDescription(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetScroll(index); 
        return item.description;
    }
    
    public string GetName(int index)
    { 
        return playerScrolls[index].name;
    }

    
    public Sprite GetSprite(string scrollName)
    {
        int index = GetIndex(scrollName);
        PlayerScrolls item = GetScroll(index); 
        return scrollSprites[index];
    }

    [Serializable]
    public class PlayerScrolls 
    {
        public string name;
        public string description;
    }
}

