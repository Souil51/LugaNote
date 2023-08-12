using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<PianoNote, AudioClip> _allNotesAudioClip;
    private static Dictionary<string, AudioClip> _commonSoundAudioClip;

    public static void LoadAllSounds()
    {
        _commonSoundAudioClip = new Dictionary<string, AudioClip>();

        LoadAllNotes();

        var commonSoundsList = Resources.LoadAll("sounds/common", typeof(AudioClip));
        foreach(var clip in commonSoundsList)
        {
            if(clip is AudioClip audioClip)
            {
                _commonSoundAudioClip.Add(audioClip.name, audioClip);
            }
        }
    }

    public static void LoadAllNotes()
    {
        _allNotesAudioClip = new Dictionary<PianoNote, AudioClip>();
        for (int i = (int)MusicHelper.LowerNote; i < (int)MusicHelper.HigherNote; i++)
        {
            var clip = (AudioClip)Resources.Load(StaticResource.RESOURCES_SOUND_NOTE_BASE + (i + 1));
            _allNotesAudioClip.Add((PianoNote)i, clip);
        }
    }

    public static void PlayNote(PianoNote note)
    {
        if (_allNotesAudioClip == null) LoadAllNotes();

        PlaySound(GetNoteClip(note));
    }

    public static void PlaySound(string audioClip, float volume = 1f)
    {
        if (_commonSoundAudioClip.ContainsKey(audioClip))
        {
            PlaySound(_commonSoundAudioClip[audioClip], volume);
        }
    }

    public static void PlaySound(AudioClip audioClip, float volume = 1f)
    {
        // Use custom One Shot Sound because PlayClipAtPoint stops working when spamming
        GameObject newGo = new GameObject();
        newGo.transform.position = Vector3.zero;
        var audioSource = newGo.AddComponent<AudioSource>();
        var oneShotSound = newGo.AddComponent<OneShotSound>();

        oneShotSound.InitializeAudioSource(audioSource);
        oneShotSound.PlayClip(audioClip, volume);
    }

    public static AudioClip GetNoteClip(PianoNote note)
    {
        AudioClip clip = null;
        _allNotesAudioClip.TryGetValue(note, out clip);
        return clip;
    }
}
