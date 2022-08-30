using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimTest : MonoBehaviour
{
    private Animator charAnimator;
    // Start is called before the first frame update
    void Start()
    {
        charAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            charAnimator.SetTrigger("EatTr");
        }
        
        if (Input.GetKeyDown("x"))
        {
            charAnimator.SetTrigger("PoisonTr");
        }
        
        if (Input.GetKeyDown("c"))
        {
            charAnimator.SetTrigger("TauntTr");
        }
        
        if (Input.GetKeyDown("v"))
        {
            charAnimator.SetTrigger("OrderTr");
        }
    }
}
