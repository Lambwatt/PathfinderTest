using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    Dictionary<int, List<SquareStatus>> Reservations;

    private void Awake()
    {
        Reservations = new Dictionary<int, List<SquareStatus>>();
    }
}

public class SquareStatus
{
    public Object PathWalker;
    public BoardSquareStatus Status;
    public int StartTime;
    public int EndTime;
}

public enum BoardSquareStatus
{
    EMPTY,
    FILLED,
    RESERVED
}