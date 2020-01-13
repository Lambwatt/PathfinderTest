using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    PathAgent SelectedUnit;
    Board Board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move all keyboard commands here
        //while moving, walk along path otherwise wait
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseInWorld = Board.GetMouseCoordinates();
            SelectedUnit.JumpTo( Board.GetBoardCoordinates(mouseInWorld));
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseInWorld = Board.GetMouseCoordinates();
            SelectedUnit.GetPath(Board.GetBoardCoordinates(mouseInWorld));
        }
    }
}
