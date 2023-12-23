using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<PianoNote, AudioClip> _allNotesAudioClip;
    private static Dictionary<string, AudioClip> _commonSoundAudioClip;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static Dictionary<PianoNote, int> _androidAllNotesAudioClip;
    private static Dictionary<string, int> _androidCommonSoundAudioClip;
#endif

    public static void LoadAllSounds()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_allNotesAudioClip == null && _commonSoundAudioClip == null)
            AndroidNativeAudio.makePool();
#endif

        _commonSoundAudioClip = new Dictionary<string, AudioClip>();
#if UNITY_ANDROID && !UNITY_EDITOR
        _androidCommonSoundAudioClip = new Dictionary<string, int>();
#endif

        LoadAllNotes();

        var commonSoundsList = Resources.LoadAll("sounds/common", typeof(AudioClip));
        foreach(var clip in commonSoundsList)
        {
            if(clip is AudioClip audioClip)
            {
                _commonSoundAudioClip.Add(audioClip.name, audioClip);

                int fileID = AndroidNativeAudio.load("common/" + clip.name + ".wav");
#if UNITY_ANDROID && !UNITY_EDITOR
                _androidCommonSoundAudioClip.Add(audioClip.name, fileID);
#endif
            }
        }
    }

    public static void LoadAllNotes()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_allNotesAudioClip == null && _commonSoundAudioClip == null)
            AndroidNativeAudio.makePool();
#endif

        _allNotesAudioClip = new Dictionary<PianoNote, AudioClip>();
#if UNITY_ANDROID && !UNITY_EDITOR
        _androidAllNotesAudioClip = new Dictionary<PianoNote, int>();
#endif

        for (int i = (int)MusicHelper.LowerNote; i < (int)MusicHelper.HigherNote; i++)
        {
            var clip = (AudioClip)Resources.Load(StaticResource.RESOURCES_SOUND_NOTE_BASE + (i + 1));
            _allNotesAudioClip.Add((PianoNote)i, clip);

#if UNITY_ANDROID && !UNITY_EDITOR
            int fileID = AndroidNativeAudio.load("note/" + clip.name + ".wav");
            _androidAllNotesAudioClip.Add((PianoNote)i, fileID);
#endif
        }
    }

    public static void PlayNote(PianoNote note)
    {
        if (_allNotesAudioClip == null) LoadAllNotes();

#if UNITY_ANDROID && !UNITY_EDITOR
        PlaySound(GetNoteFileID(note), 1f);
#else
        PlaySound(GetNoteClip(note), 1f);
#endif
    }

    public static void PlaySound(string audioClip, float volume = 1f)
    {
        if (_commonSoundAudioClip == null) LoadAllSounds();

        if (_commonSoundAudioClip.ContainsKey(audioClip))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            PlaySound(_androidCommonSoundAudioClip[audioClip], volume);
#else
            PlaySound(_commonSoundAudioClip[audioClip], volume);
#endif
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void PlaySound(int fileID, float volume)
    {
        AndroidNativeAudio.play(fileID);
    }
#else
    private static void PlaySound(AudioClip audioClip, float volume = 1f)
    {
        // Use custom One Shot Sound because PlayClipAtPoint stops working when spamming
        GameObject newGo = new GameObject();
        newGo.transform.position = Vector3.zero;
        var audioSource = newGo.AddComponent<AudioSource>();
        var oneShotSound = newGo.AddComponent<OneShotSound>();

        oneShotSound.InitializeAudioSource(audioSource);
        oneShotSound.PlayClip(audioClip, volume);
    }
#endif

    public static AudioClip GetNoteClip(PianoNote note)
    {
        AudioClip clip = null;
        _allNotesAudioClip.TryGetValue(note, out clip);
        return clip;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public static int GetNoteFileID(PianoNote note)
    {
        int id = 0;
        _androidAllNotesAudioClip.TryGetValue(note, out id);
        return id;
    }
#endif
}
