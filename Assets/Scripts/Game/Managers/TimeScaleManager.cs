using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private float _lastTimeScale;
    private float _pauseTimer;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    // Update is called once per frame
    void Update()
    {
        if (IsPaused)
            UpdatePause();

        var firstNote = GameController.Instance.GetFirstNote();
        // Update the timescale to slow down notes while they are approching the start of the staff
        if (firstNote != null && !GameController.Instance.IsPaused && !GameController.Instance.IsGameEnded)
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

    /// <summary>
    /// Pause the game, setting Timescale to 0f and saving timescale before the pause
    /// </summary>
    public void PauseGame(float duration)
    {
        _isPaused = true;
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _pauseTimer = duration;
    }

    /// <summary>
    /// Update the pause timer and unpause if the timer is ended
    /// </summary>
    private void UpdatePause()
    {
        if (_pauseTimer != 0) // Pause can be positive or negative (= unlimited pause)
        {
            if (_pauseTimer > 0)
            {
                _pauseTimer -= Time.unscaledDeltaTime;
                if (_pauseTimer < 0)
                {
                    UnpauseGame();
                }
            }

            return;
        }
    }


    /// <summary>
    /// Resume the game after a pause
    /// </summary>
    public void UnpauseGame()
    {
        _pauseTimer = 0f;
        Time.timeScale = _lastTimeScale;
        _isPaused = false;
    }

    public void UnpauseGameAfterSeconds(float seconds)
    {
        _pauseTimer = seconds;
    }
}
