using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to help generating Staff at loading
/// 2 of this are used to determine the position of all lines in the Staff
/// These 2 are placed manually on each Clef
/// </summary>
public class LinePositionIndicator : MonoBehaviour
{
    [SerializeField] private int Position;

    public int GetPosition()
    {
        return Position;
    }
}
