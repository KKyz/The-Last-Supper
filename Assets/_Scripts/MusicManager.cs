using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip winResultBGM, loseJingle, winJingle, loseResultsBGM;
    
    [HideInInspector]
    public AudioSource musicPlayer;

    private void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.loop = true;
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
