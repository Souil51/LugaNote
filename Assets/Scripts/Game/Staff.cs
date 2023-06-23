using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Staff script
/// Handle StaffLine spawn
/// </summary>
public class Staff : MonoBehaviour
{
    [SerializeField] private List<LinePositionIndicator> Indicators = new List<LinePositionIndicator>();
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

    private float _lineDistance = 0f;
    public float LineDistance => _lineDistance;

    public List<Note> Notes 
    {
        get
        {
            var allNotes = Lines.SelectMany(x => x.Notes);
            return allNotes.OrderBy(x => x.transform.position.x).ToList();
        }    
    }
    private List<StaffLine> Lines = new List<StaffLine>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        float yHalfDistance = yDistance / 2;
        _lineDistance = yHalfDistance;

        // Starting at the top line
        float currentY = firstIndicator.transform.position.y - (((minPosition * 2) + 7) * yHalfDistance);

        PianoNote currentNote = MusicHelper.GetFirstPianoNoteForClef(Clef);

        // Instantiating 23 lines
        for(int i = 0; i < 23; i++)
        {
            var goLine = Instantiate(Resources.Load(StaticResource.PREFAB_LINE)) as GameObject;
            goLine.transform.position = new Vector3(0, currentY, 0);

            var staffLine = goLine.GetComponent<StaffLine>();
            staffLine.InitializeLine(this, i, currentNote, i >= 7 && i <= 15, i % 2 == 0);

            Lines.Add(staffLine);

            currentY += yHalfDistance;

            currentNote++;
            if (MusicHelper.SharpNotes.Contains(currentNote))
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
        _availableLines[index].SpawnNote(transform.localScale.x, StartingPointPosition, DisappearPointPosition);

        return index;
    }
}