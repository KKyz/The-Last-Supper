using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVomit : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DisableSplash());
    }

    private IEnumerator DisableSplash()
    {
        LeanTween.alpha(gameObject, 0, 0.5f);
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }

}
