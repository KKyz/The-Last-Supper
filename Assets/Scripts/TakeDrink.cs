using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDrink : MonoBehaviour
{
    private PlayerFunctions playerF;

    public void Start()
    {
        playerF = GameObject.Find("PlayerCanvas").GetComponent<PlayerFunctions>();
    }

    public void Drink(int i)
    {
        if (playerF.player.psnArray[i])
        {
            playerF.CmdPoison(true);
        }
        playerF.RpcResetActions();
        gameObject.GetComponent<SpawnMenu>().SlideOutMenu();
    }
}
