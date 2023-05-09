using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StaffLine : MonoBehaviour
{
    private List<Note> _notes = new List<Note>();
    public List<Note> Notes => _notes;
    private bool _isSpaceLine;
    private bool _isVisible;
    private int _id;

    private SpriteRenderer _sprtRenderer;

    private void Awake()
    {
        _sprtRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeLine(int id, bool visible, bool spaceLine)
    {
        this._isSpaceLine = spaceLine;
        this._isVisible = visible;
        this._id = id;

        if(!_isVisible || _isSpaceLine)
        {
            _sprtRenderer.sprite = null;
        }
    }

    /// <summary>
    /// Instantiate a note on the line starting position to the line ending position
    /// </summary>
    public void SpawnNote(Vector3 from, Vector3 to)
    {
        string resourceToLoad = !_isVisible && !_isSpaceLine ? StaticResource.PREFAB_NOTE_LINE : StaticResource.PREFAB_NOTE_NO_LINE;

        GameObject go = (GameObject)Instantiate(Resources.Load(resourceToLoad));
        go.transform.position = new Vector3(from.x, transform.position.y, transform.position.z);

        var note = go.GetComponent<Note>();
        note.MoveTo(new Vector3(to.x, transform.position.y, transform.position.z));

        _notes.Add(note);
    }
}
