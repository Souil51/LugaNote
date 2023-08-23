using Guid = System.Guid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor.Localization.Plugins.XLIFF.V20;

/// <summary>
/// Staff script
/// Handle StaffLine spawn
/// </summary>
public class Staff : MonoBehaviour
{
    [SerializeField] private List<LinePositionIndicator> Indicators = new();
    [SerializeField] private Clef Clef;

    public Clef StaffClef => Clef;

    private SpriteRenderer spriteRenderer;
    public float SpriteWidth => spriteRenderer.size.x * transform.localScale.x;

    private float _startingPointPosition = 0f;
    public float StartingPointPosition => _startingPointPosition;

    private float _endingPointPosition = 0f;
    public float EndingPointPosition => _endingPointPosition;

    private float _disappearPointPosition = 0f;
    public float DisappearPointPosition => _disappearPointPosition;

    private float _lineDistanceSpacing = 0f;
    public float LineDistanceSpacing => _lineDistanceSpacing;

    public List<Note> Notes 
    {
        get
        {
            var allNotes = Lines.SelectMany(x => x.Notes);
            return allNotes.OrderBy(x => x.transform.position.x).ToList();
        }    
    }
    private readonly List<StaffLine> Lines = new();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeStaff()
    {
        InitializeClef();

        var firstLine = Lines[0];
        _startingPointPosition = firstLine.Width / 2;
        _disappearPointPosition = -(firstLine.Width / 2f);
        _endingPointPosition = _disappearPointPosition + SpriteWidth;
    }

    /// <summary>
    /// Generate all the lines for this staff using position indicators the calculate the starting position and the distance bewteen 2 lines
    /// </summary>
    public void InitializeClef()
    {
        // Search of the distance between each lines using Indicators transform position
        var orderedIndicators = Indicators.OrderBy(x => x.GetPosition());
        var firstIndicator = orderedIndicators.First();
        var lastIndicator = orderedIndicators.Last();

        int minPosition = firstIndicator.GetPosition();
        int maxPosition = lastIndicator.GetPosition();

        int positionDifference = maxPosition - minPosition;

        float yDistance = (firstIndicator.transform.position.y - lastIndicator.transform.position.y) / positionDifference;
        float yHalfSpacing = yDistance / 2;
        _lineDistanceSpacing = yHalfSpacing;

        // Starting at the top line
        float currentY = firstIndicator.transform.position.y - (((minPosition * 2) + 7) * yHalfSpacing);

        PianoNote currentNote = MusicHelper.GetFirstPianoNoteForClef(Clef);

        // Instantiating 23 lines
        for(int i = 0; i < 23; i++)
        {
            var goLine = Instantiate(Resources.Load(StaticResource.PREFAB_LINE)) as GameObject;
            goLine.transform.position = new Vector3(0, currentY, 0);

            var staffLine = goLine.GetComponent<StaffLine>();
            staffLine.InitializeLine(this, currentNote, i >= 7 && i <= 15, i % 2 == 0);

            Lines.Add(staffLine);

            currentY += yHalfSpacing;

            currentNote++;
            if (MusicHelper.IsSharp(currentNote))
                currentNote++;
        }

        
    }

    /// <summary>
    /// Instantiate a note on a random line
    /// </summary>
    /// <returns>The line position from top to bottom, starting at 0</returns>
    public int SpawnNote(PianoNote higherNote = PianoNote.C8, PianoNote lowerNote = PianoNote.A0)
    {
        var _availableLines = Lines.Where(x => x.Note >= lowerNote && x.Note <= higherNote).ToList();

        int index = Random.Range(0, _availableLines.Count);
        _availableLines[index].SpawnRandomNote(transform.localScale.x, StartingPointPosition, DisappearPointPosition);

        return index;
    }

    public int SpawnNote(List<PianoNote> notes, bool withRandomAccidental = false)
    {
        var _availableLines = Lines.Where(x => notes.Contains(x.Note)).ToList();
        /*var _availableLines = new List<StaffLine>();
        _availableLines.Add(Lines.First());
        _availableLines.Add(Lines.Last());*/

        int index = Random.Range(0, _availableLines.Count);

        if(withRandomAccidental)
            _availableLines[index].SpawnRandomNoteWithRandomAccidental(transform.localScale.x, StartingPointPosition, DisappearPointPosition);
        else
            _availableLines[index].SpawnRandomNote(transform.localScale.x, StartingPointPosition, DisappearPointPosition);

        return index;
    }

    public int SpawnNote(List<PianoNote> notes, Accidental accidental)
    {
        var _availableLines = Lines.Where(x => notes.Contains(x.Note)).ToList();

        int index = Random.Range(0, _availableLines.Count);
        _availableLines[index].SpawnRandomNoteWithAccidental(transform.localScale.x, StartingPointPosition, DisappearPointPosition, accidental);

        return index;
    }

    public void SpawnMultipleNotes(int count, List<PianoNote> notesRange, bool withRandomAccidental = false)
    {
        Guid groupId = Guid.NewGuid();
        var _availableLines = Lines.Where(x => notesRange.Contains(x.Note)).ToList();

        PianoNote maxNote = PianoNote.C8;
        PianoNote minNote = PianoNote.A0;

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, _availableLines.Count);
            var staffLine = _availableLines[index];

            if (withRandomAccidental)
                staffLine.SpawnRandomNoteWithRandomAccidental(transform.localScale.x, StartingPointPosition, DisappearPointPosition, groupId);
            else
                staffLine.SpawnRandomNote(transform.localScale.x, StartingPointPosition, DisappearPointPosition, groupId);

            _availableLines.RemoveAt(index);

            if (i == 0)
            {
                minNote = (int)staffLine.Note - 12 >= 0 ? staffLine.Note - 12 : PianoNote.A0;
                maxNote = (int)staffLine.Note + 12 <= (int)PianoNote.C8 ? staffLine.Note + 12 : PianoNote.C8;
                _availableLines = _availableLines.Where(x => x.Note >= minNote && x.Note <= maxNote).ToList();
            }
        }
    }

    public void SpawnChord(List<PianoNote> notesRange, bool withAccidental, bool withInversion)
    {
        Guid groupId = Guid.NewGuid();
        var majorChords = MusicHelper.GetMajorChords(notesRange.Min(), notesRange.Max(), withAccidental, withInversion);
        var minorChords = MusicHelper.GetMinorChords(notesRange.Min(), notesRange.Max(), withAccidental, withInversion);
        var chords = majorChords.Concat(minorChords).ToList();

        int index = Random.Range(0, chords.Count);

        var chordToPlay = chords[index].Notes;
        var chordAllNatural = chordToPlay.Select(x => MusicHelper.ConvertToNaturalNote(x)).ToList();

        foreach(var note in chordToPlay)
        {
            Debug.Log(note);
            var line = Lines.Where(x => x.NaturalNote == MusicHelper.ConvertToNaturalNote(note)).First();
            line.SpawnNote(note, transform.localScale.x, StartingPointPosition, DisappearPointPosition, groupId);
        }
    }
}
