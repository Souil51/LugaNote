using MidiJack;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject prefab;
    public TextMeshProUGUI text;

    void Start()
    {
        for (var i = 0; i < 128; i++)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.position = new Vector3(i % 12, i / 12, 0);
            go.GetComponent<NoteIndicator>().noteNumber = i;

#if UNITY_ANDROID && !UNITY_EDITOR
            text.text = "ANDROID";
#endif
        }

        MidiMaster.noteOnDelegate += NoteOn;
    }

    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        AskPermission();
    }

    private void AskPermission()
    {
#if UNITY_ANDROID

#endif

        // AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject context = new AndroidJavaClass("com.helagos.androidutilspermission.AndroidUtils");

        int i = 0;
    }
}
