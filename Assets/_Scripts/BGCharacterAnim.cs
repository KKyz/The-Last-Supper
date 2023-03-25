using System.Collections;
using UnityEngine;

public class BGCharacterAnim : MonoBehaviour
{
    private bool nextAnimation;
    private Animator anim;
    private Vector3 startPos;
    private Quaternion rotationPos;

    void Start()
    {
        startPos = transform.position;
        rotationPos = transform.rotation;
        nextAnimation = true;
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (nextAnimation)
        {
            StartCoroutine(PlayRandomAnim());
        }
    }

    private IEnumerator PlayRandomAnim()
    {
        nextAnimation = false;
        
        yield return new WaitForSeconds(1f);
        
        int randIndex = Random.Range(1, 12);
        anim.SetTrigger(randIndex.ToString());

        yield return new WaitForSeconds(Random.Range(6f, 10f));
        
        anim.SetTrigger("7"); // Reset to Active Anim
        transform.position = startPos;
        transform.rotation = rotationPos;
        
        nextAnimation = true;
    }
}
