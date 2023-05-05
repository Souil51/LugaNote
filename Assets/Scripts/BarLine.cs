using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarLine : MonoBehaviour
{
    [SerializeField] private GameObject StartingPoint;
    [SerializeField] private GameObject EndingPoint;

    private List<Note> _notes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNote()
    {
        GameObject go = (GameObject)Instantiate(Resources.Load("Note"));
        go.transform.position = StartingPoint.transform.position;

        var note = go.GetComponent<Note>();
        note.MoveTo(EndingPoint.transform.position);
    }
}
