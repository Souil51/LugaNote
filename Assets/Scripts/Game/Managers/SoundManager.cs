using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static void PlayNote(PianoNote note)
    {
        int noteIndex = (int)note + 1;
        AudioClip clip = (AudioClip)Resources.Load(StaticResource.RESOURCES_SOUND_NOTE_BASE + noteIndex);
        PlaySound(clip);
    }

    public static void PlaySound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
    }
}
