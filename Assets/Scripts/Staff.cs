using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Staff : MonoBehaviour
{
    [SerializeField] private List<LinePositionIndicator> Indicators = new List<LinePositionIndicator>();

    private List<StaffLine> Lines = new List<StaffLine>();

    private void Awake()
    {
        InitializeClef();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        // Starting at the top line
        float currentY = firstIndicator.transform.position.y - (((minPosition * 2) + 7) * yHalfDistance);

        // Instantiating 23 lines
        for(int i = 0; i < 23; i++)
        {
            var goLine = Instantiate(Resources.Load("Line")) as GameObject;
            goLine.transform.position = new Vector3(0, currentY, 0);

            Lines.Add(goLine.GetComponent<StaffLine>());

            currentY += yDistance;
        }
    }

    /// <summary>
    /// Instantiate a note on a random line
    /// </summary>
    /// <returns>The line position from top to bottom, starting at 0</returns>
    public int SpawnNote()
    {
        int index = Random.Range(0, Lines.Count);
        Lines[index].SpawnNote();

        return index;
    }
}
