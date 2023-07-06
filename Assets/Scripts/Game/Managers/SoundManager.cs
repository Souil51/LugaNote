using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<PianoNote, AudioClip> _allNotesAudioClip;

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

    public static void PlaySound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
    }

    public static AudioClip GetNoteClip(PianoNote note)
    {
        AudioClip clip = null;
        _allNotesAudioClip.TryGetValue(note, out clip);
        return clip;
    }
}
