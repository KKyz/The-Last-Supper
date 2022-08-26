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
        if (Input.GetKeyDown("1"))
        {
            charAnimator.Play("Clerk1|defaultsittingpose");
        }
        
        if (Input.GetKeyDown("2"))
        {
            charAnimator.Play("Clerk1|active");
        }
        
        if (Input.GetKeyDown("3"))
        {
            charAnimator.Play("Clerk1|Poison");  
        }
    }
}
