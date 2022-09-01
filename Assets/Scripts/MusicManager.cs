using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip titleBGM, winBGM, loseBGM, resultsBGM;
    private AudioSource musicPlayer;
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.loop = true;

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("StartMenu"))
        {
            musicPlayer.clip = titleBGM;
            musicPlayer.Play();
        }
    }

    public void PlayBGM(AudioClip courseBGM)
    {
        if (musicPlayer.clip != courseBGM)
        {
            StartCoroutine(BGMFadeOut(musicPlayer, 0.5f, courseBGM));
        }
    }
    
    public IEnumerator PlayResultBGM(bool hasWon)
    {
        musicPlayer.loop = false;
        if (hasWon)
        {
            musicPlayer.clip = winBGM;
            musicPlayer.Play();
        }

        else
        {
            musicPlayer.clip = loseBGM;
            musicPlayer.Play();
        }
        
        yield return new WaitForSeconds(2f);
        
        musicPlayer.clip = resultsBGM;
        musicPlayer.Play();
    }
    
    public IEnumerator BGMFadeOut (AudioSource audioSource, float fadeTime, AudioClip nextBGM) 
    {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;

        yield return new WaitForSeconds(0.3f);
        
        musicPlayer.clip = nextBGM;
        musicPlayer.Play();
    }
}
