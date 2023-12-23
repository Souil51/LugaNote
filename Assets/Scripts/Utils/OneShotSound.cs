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

    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        StartCoroutine(Co_PlayClip(clip, volume));
    }

    public void PlayClipAndroid(AudioClip clip, string basePath, float volume = 1f)
    {
        StartCoroutine(Co_PlayClipAndroid(clip, basePath, volume));
    }

    private IEnumerator Co_PlayClip(AudioClip clip, float volume = 1f)
    {
        if (_audioSource.isPlaying) _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.Play();

        yield return new WaitForSecondsRealtime(clip.length + 0.1f);

        Destroy(this.gameObject);
    }

    private IEnumerator Co_PlayClipAndroid(AudioClip clip, string basePath, float volume = 1f)
    {
        //Debug.Log("PLAY SOUND ANDROID");
        int fileId = AndroidNativeAudio.load(basePath + "/" + clip.name + ".wav");
        AndroidNativeAudio.play(fileId);

        yield return new WaitForSecondsRealtime(clip.length + 0.1f);

        Destroy(this.gameObject);
    }
}
