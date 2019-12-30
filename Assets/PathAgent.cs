using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    public PathFinder pathFinder;
    public Board board;
    public GameObject PathMarker;

    public Vector2Int Location;
    public Vector2Int Destination;

    List<GameObject> PathMarks;
    List<Vector2Int> path;
    PositionAppointment pathAppointments; 

    // Start is called before the first frame update
    void Start()
    {
        PathMarks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //while moving, walk along path otherwise wait
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseInWorld = board.GetMouseCoordinates();
            Location = board.GetBoardCoordinates(mouseInWorld);
            transform.position = board.ConvertToWorldCoordinates(Location);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            clearPath();

            Vector3 mouseInWorld = board.GetMouseCoordinates();
            Destination = board.GetBoardCoordinates(mouseInWorld);

            path = pathFinder.GetPath(Location, Destination);

            //If successful, plot path, otherwise stop
            if (path != null)
            {

                //Create Path
                for (int i = 0; i < path.Count; i++)
                {
                    PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates(path[i]) + new Vector3(0, 2, 0), Quaternion.identity));
                    if (i > 0)
                    {
                        PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)path[i - 1] + ((Vector2)path[i] - (Vector2)path[i - 1]) * .25f) + new Vector3(0, 2, 0), Quaternion.identity));
                        PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)path[i - 1] + ((Vector2)path[i] - (Vector2)path[i - 1]) * .5f) + new Vector3(0, 2, 0), Quaternion.identity));
                        PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)path[i - 1] + ((Vector2)path[i] - (Vector2)path[i - 1]) * .75f) + new Vector3(0, 2, 0), Quaternion.identity));
                    }
                }
            }
            else
            {
                Debug.Log("Could not find path");
            }
        }
    }

    void clearPath()
    {
        for (int i = PathMarks.Count - 1; i >= 0; i--)
        {
            Destroy(PathMarks[i]);
        }
        PathMarks.Clear();
    }

    public void Walk()
    {
        //Begin walking along path
    }
}
