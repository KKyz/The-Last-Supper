using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCharacterAnim : MonoBehaviour
{
    private bool isInAnimation;
    private Animator anim;
    public List<string> animList = new();

    void Start()
    {
        isInAnimation = false;
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (!isInAnimation)
        {
            StartCoroutine(PlayRandomAnim());
        }
    }

    private IEnumerator PlayRandomAnim()
    {
        isInAnimation = true;
        string index = animList[Random.Range(0, animList.Count)];
        anim.SetTrigger(index);

        yield return new WaitForSeconds(6f);
        
        isInAnimation = false; 
    }
}
