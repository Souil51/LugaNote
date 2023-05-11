using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Staff Staff;

    public bool IsStopped => Time.timeScale != 0f;

    private int _points = 0;

    private void Awake()
    {
        StartCoroutine(Co_SpawnNotes());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();

        if (firstNote != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                firstNote.SetInactive();
                _points++;

                firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();
            }

            if (firstNote != null)
            {
                var totalDistance = Staff.StartingPointPosition - Staff.EndingPointPosition;
                var distanceToEnd = firstNote.transform.position.x - Staff.EndingPointPosition;

                float newTimeScale = distanceToEnd / totalDistance;
                if (newTimeScale > 0.05f)
                    Time.timeScale = distanceToEnd / totalDistance;
                else
                    Time.timeScale = 0f;
            }
        }

        // Debug.Log("Timescale : " + Time.timeScale);
    }

    public IEnumerator Co_SpawnNotes()
    {
        while (true)
        {
            Staff.SpawnNote();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
