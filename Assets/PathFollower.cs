using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Board board;

    public Vector2Int Location;
    public Vector2Int Destination;

    public GameObject PathMarker;

    List<GameObject> PathMarks; 

    // Start is called before the first frame update
    void Start()
    {
        PathMarks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseInWorld = board.GetMouseCoordinates();
            Location = board.GetBoardCoordinates(mouseInWorld);
            transform.position = board.ConvertToWorldCoordinates(Location);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseInWorld = board.GetMouseCoordinates();
            Destination = board.GetBoardCoordinates(mouseInWorld);
            //List<Vector2Int> spots = CreatePath(Destination, Location);
            List<Vector2Int> spots = new List<Vector2Int>();

            List<PathRay> steps = CreatePath2(Location, Destination);
            Stack<Vector2Int> tmp = new Stack<Vector2Int>();
            
            foreach(PathRay step in steps)
            {
                tmp = step.GetPath();
                while (tmp.Count > 0) {
                    spots.Add(tmp.Pop());
                }
            }

            //Clear path
            for (int i = PathMarks.Count - 1; i>=0; i--)
            {
                Destroy(PathMarks[i]);
                //PathMarks.RemoveAt(i);
            }
            PathMarks.Clear();
            
            //Create Path
            for(int i = 0; i<spots.Count; i++)
            {
                PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates(spots[i]) + new Vector3(0, 2, 0), Quaternion.identity));
            }
        }
    }

    List<Vector2Int> CreatePath(Vector2Int destination, Vector2Int location)
    {
        Debug.Log("Searching for path from "+destination+" to " +location);
        Vector2Int diff = new Vector2Int(destination.x - location.x, destination.y - location.y);
        float theta = Mathf.Atan2(diff.y, diff.x);
        Debug.Log("Found theta: "+theta);
        return CreatePath(destination, location, new List<Vector2Int>(), theta, new Vector2(), 0);
    }

    List<Vector2Int> CreatePath(Vector2Int destination, Vector2Int location, List<Vector2Int> path, float theta, Vector2 remainder, int iterations)
    {
        if (iterations > 20)
        {
            return path;
        }

        if (destination.Equals(location))
        {
            return path;
        }
        else
        {
            if(Mathf.Abs(remainder.x) > 1)
            {
                Vector2Int addition = new Vector2Int(Mathf.RoundToInt(Mathf.Sign(remainder.x)), 0);
                path.Add(location + addition);
                remainder.x -= Mathf.Sign(remainder.x);
                return CreatePath(destination, location + addition, path, theta, remainder, iterations+1);
            }
            else if(Mathf.Abs(remainder.y) > 1)
            {                
                Vector2Int addition = new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(remainder.y)));
                path.Add(location + addition);
                remainder.y -= Mathf.Sign(remainder.y);
                return CreatePath(destination, location + addition, path, theta, remainder, iterations + 1);
            }
            else
            {
                remainder += new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                return CreatePath(destination, location, path, theta, remainder, iterations + 1);
            }
        }
    }

    public List<PathRay> CreatePath2(Vector2Int destination, Vector2Int location)
    {
        int Iterations = 0;

        //base case
        if (location == destination)
            return null;

        List<PathRay> steps = new List<PathRay>();
        PathRay ray = new PathRay(location, destination);

        steps.Add(ray);

        do
        {
            Iterations += 1;
            while (ray.Step());
            //Check for obstruction and do recursive things
            
        } while (ray.Location() != destination && Iterations<20);
        
        //Other base case. destination reached
        return steps;
    }

    //Walks in a straight line from a point
    public class PathRay
    {
        Stack<Vector2Int> Path;

        Vector2Int Start;
        Vector2Int DominantVector;
        Vector2Int SecondaryVector;
        Vector2Int PendingVector;

        Vector2 remainder;
        Vector2 diff;
        
        int Iterations = 0;
        float theta;
        bool hasPending = false;

        public PathRay(Vector2Int start, Vector2Int end)
        {
            Start = start;
            diff = end - start;
            remainder = new Vector2(0,0);
            theta = Mathf.Atan2(diff.y, diff.x);

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                DominantVector = new Vector2Int(1, 0);
                SecondaryVector = new Vector2Int(0, 1);
            }
            else
            {
                DominantVector = new Vector2Int(0, 1);
                SecondaryVector = new Vector2Int(1, 0);
            }

            Path = new Stack<Vector2Int>();
        }

        //returns true while step is incomplete
        public bool Step()
        {
            Vector2Int addition = new Vector2Int(0, 0);
            if (new Vector2(remainder.x * DominantVector.x, remainder.y * DominantVector.y).magnitude > 1)
            {
                addition += new Vector2Int(Mathf.RoundToInt(Mathf.Sign(remainder.x) * DominantVector.x), Mathf.RoundToInt(Mathf.Sign(diff.y) * DominantVector.y));
                remainder -= new Vector2Int(Mathf.RoundToInt(Mathf.Sign(remainder.x) * DominantVector.x), Mathf.RoundToInt(Mathf.Sign(diff.y) * DominantVector.y));

                if (new Vector2(remainder.x * SecondaryVector.x, remainder.y * SecondaryVector.y).magnitude > 1 /*&& !hasPending*/)
                {
                    addition += new Vector2Int(Mathf.RoundToInt(Mathf.Sign(remainder.x) * SecondaryVector.x), Mathf.RoundToInt(Mathf.Sign(diff.y) * SecondaryVector.y));
                    remainder -= new Vector2Int(Mathf.RoundToInt(Mathf.Sign(remainder.x) * SecondaryVector.x), Mathf.RoundToInt(Mathf.Sign(diff.y) * SecondaryVector.y));
                }
            }

            if (addition.magnitude > 0)
            {
                Path.Push(Start += addition);
                return false;
            }

            remainder += new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            return true;
        }

        public Vector2Int Location()
        {
            return Path.Peek();
        }

        public Stack<Vector2Int> GetPath()
        {
            Stack<Vector2Int> res = new Stack<Vector2Int>();
            while (Path.Count > 0)
            {
                res.Push(Path.Pop());
            }
            return res;
        }
    }
}
