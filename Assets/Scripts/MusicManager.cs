using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip titleBGM;
    private AudioSource musicPlayer;
    public AudioMixer bgmMixer;
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("StartMenu"))
        {
            musicPlayer.clip = titleBGM;
            musicPlayer.Play();
        }
    }

    public void PlayBGM(AudioClip courseBGM)
    {
        if (courseBGM != null)
        {
            if (musicPlayer.clip != null)
            {
                StartCoroutine(BGMFadeOut(0.5f));
            }
            //Fade out current audio
            //Start next audio
            bgmMixer.SetFloat("BGMParam", 1f);
            musicPlayer.clip = courseBGM;
            musicPlayer.Play();
        }
    }
    
    public IEnumerator BGMFadeOut(float duration)
    {
        float currentTime = 0;
        float currentVol;
        bgmMixer.GetFloat("BGMParam", out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(0, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            bgmMixer.SetFloat("BGMParam", Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }
}
