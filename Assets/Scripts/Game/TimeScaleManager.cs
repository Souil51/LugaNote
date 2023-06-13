using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var firstNote = GameController.Instance.GetFirstNote();
        // Update the timescale to slow down notes while they are approching the start of the staff
        if (firstNote != null)
        {
            var firstNoteStaff = firstNote.Parent.Parent;
            // timescale based on the first Staff
            var totalDistance = firstNoteStaff.StartingPointPosition - firstNoteStaff.EndingPointPosition;
            var distanceToEnd = firstNote.transform.localPosition.x - firstNoteStaff.EndingPointPosition;

            float newTimeScale = distanceToEnd / totalDistance;
            if (newTimeScale > 0.05f) // deadzone
                Time.timeScale = distanceToEnd / totalDistance;
            else
                Time.timeScale = 0f;
        }
    }
}
