using UnityEngine;
using MidiJack;

public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;

    void Update()
    {
        transform.localScale = Vector3.one * (0.1f + MidiMaster.GetKey(noteNumber));

        bool keyDown = MidiMaster.GetKeyDown(noteNumber);

        if (keyDown)
        {
            int i = 0;
        }

        var color = MidiMaster.GetKeyDown(noteNumber) ? Color.red : Color.white;
        GetComponent<Renderer>().material.color = color;
    }
}
