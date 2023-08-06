using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneShotSound : MonoBehaviour
{
    private AudioSource _audioSource;
    public void InitializeAudioSource(AudioSource audioSource)
    {
        _audioSource = audioSource;
        _audioSource.playOnAwake = false;
    }

    public void PlayClip(AudioClip clip)
    {
        StartCoroutine(Co_PlayClip(clip)); 
    }

    private IEnumerator Co_PlayClip(AudioClip clip)
    {
        if (_audioSource.isPlaying) _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();

        yield return new WaitForSecondsRealtime(clip.length);

        Destroy(this.gameObject);
    }
}
