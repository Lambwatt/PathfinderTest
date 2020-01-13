using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public SelectionSquare Selection;
    public PathAgent Prefab;
    public Board Board;
    public PathFinder PathManager;

    List<PathAgent> AllUnits;
    PathAgent SelectedUnit;
    
    // Start is called before the first frame update
    void Start()
    {
        AllUnits = new List<PathAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //move all keyboard commands here
        //while moving, walk along path otherwise wait
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int location = Board.GetBoardCoordinates(Board.GetMouseCoordinates());

            for (int i = 0; i < AllUnits.Count; i++)
            {
                if (AllUnits[i].Location == location)
                {
                    if (SelectedUnit == AllUnits[i])
                    {
                        Selection.Deselect();
                        SelectedUnit = null;
                    }
                    else
                    {
                        SelectedUnit = AllUnits[i];
                        Selection.Select(SelectedUnit);
                    }
                    return;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseInWorld = Board.GetMouseCoordinates();
            SelectedUnit.GetPath(Board.GetBoardCoordinates(mouseInWorld));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Vector3 mouseInWorld = Board.GetMouseCoordinates();
            PathAgent unit = Instantiate(Prefab);           
            unit.board = Board;
            unit.pathFinder = PathManager;
            unit.JumpTo( Board.GetBoardCoordinates(mouseInWorld));
            AllUnits.Add(unit);
        }
    }
}
