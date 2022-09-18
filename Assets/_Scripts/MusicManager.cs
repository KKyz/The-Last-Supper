using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip titleBGM, winResultBGM, loseJingle, winJingle, loseResultsBGM;
    private AudioSource musicPlayer;

    private void Start()
    {
        PlayTitle();
    }
    
    public void PlayTitle()
    {
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.loop = true;
        
        musicPlayer.clip = titleBGM;
        musicPlayer.Play();
    }

    public void PlayBGM(AudioClip courseBGM)
    {
        if (musicPlayer.clip != courseBGM)
        {
            StartCoroutine(BGMFadeOut(musicPlayer, 0.5f, courseBGM));
        }
    }
    
    public IEnumerator PlayLoseResultBGM()
    {
        musicPlayer.Stop();

        musicPlayer.PlayOneShot(loseJingle);
        
        yield return new WaitForSeconds(6f);
    
        musicPlayer.clip = loseResultsBGM;
        musicPlayer.Play();
    }
    
    public IEnumerator PlayWinResultBGM()
    {
        musicPlayer.Stop();

        musicPlayer.PlayOneShot(winJingle);
        
        yield return new WaitForSeconds(4f);
    
        musicPlayer.clip = winResultBGM;
        musicPlayer.Play();
    }
    
    private IEnumerator BGMFadeOut (AudioSource audioSource, float fadeTime, AudioClip nextBGM) 
    {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;

        yield return new WaitForSeconds(0.8f);
        
        musicPlayer.clip = nextBGM;
        musicPlayer.Play();
    }
}
