using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Board board;

    public Vector2Int Location;
    public Vector2Int Destination;

    public GameObject PathMarker;
    public GameObject BlockMarker;

    List<GameObject> PathMarks;

    public bool[,] Obstacles;
    public GameObject[,] ObstacleObjects;

    public static Vector2Int[] directions;

    // Start is called before the first frame update
    void Start()
    {
        //if (directions != null)
        //{
            directions = new Vector2Int[] {
                new Vector2Int( 1, 0),
                new Vector2Int( 1,-1),
                new Vector2Int( 0,-1),
                new Vector2Int(-1,-1),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1)
            };

            Obstacles = new bool[board.Dimensions.x, board.Dimensions.y];
            ObstacleObjects = new GameObject[board.Dimensions.x, board.Dimensions.y];
        //}

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

            Stack<Vector2Int> tmp = new Stack<Vector2Int>();

            /*List<PathRay> steps =*/ CreatePath3(Destination, Location, ref tmp);
            //Stack<Vector2Int> tmp = new Stack<Vector2Int>();
            
            //foreach(PathRay step in steps)
            //{
            //    tmp = step.GetPath();
                while (tmp.Count > 0) {
                    spots.Add(tmp.Pop());
                }
            //}

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 mouseInWorld = board.GetMouseCoordinates();
            Vector2Int spot = board.GetBoardCoordinates(mouseInWorld);

            if (Obstacles[spot.x, spot.y])
            {
                Obstacles[spot.x,spot.y] = false;
                Destroy(ObstacleObjects[spot.x, spot.y]);
            }
            else
            {
                Obstacles[spot.x, spot.y] = true;
                ObstacleObjects[spot.x, spot.y] = Instantiate(BlockMarker, board.ConvertToWorldCoordinates(spot) + new Vector3(0, 2, 0), Quaternion.identity);
            }
        }
    }

    List<Vector2Int> CreatePath(Vector2Int destination, Vector2Int location)
    {
        //Debug.Log("Searching for path from "+destination+" to " +location);
        Vector2Int diff = new Vector2Int(destination.x - location.x, destination.y - location.y);
        float theta = Mathf.Atan2(diff.y, diff.x);
        //Debug.Log("Found theta: "+theta);
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

    //This algorithm can do 
    public List<PathRay> CreatePath2(Vector2Int destination, Vector2Int location, int back = -1)
    {
        int Iterations = 0;

        //base case
        if (location == destination || Obstacles[destination.x, destination.y])
            return null;

        List<PathRay> steps = new List<PathRay>();
        PathRay ray = new PathRay(location, destination);

        steps.Add(ray);

        do
        {
            Iterations += 1;
            while (ray.Step());
            //Check for obstruction and do recursive things
            if (Obstacles[ray.Location().x, ray.Location().y])
            {
                Debug.Log("BLOCKED!");
                int dirIndex = ray.GetDirectionIndex();
                Debug.Log("Couldn't go in direction[" + directions[dirIndex] + "]");

                int limit = Mathf.Abs( dirIndex - back );

                int index;
                Vector2Int hub = ray.Location() - directions[dirIndex];
                Vector2Int candidate;
                for (int i = 1; i<=limit || (back == -1 &&  (i<directions.Length/2) ); i++)
                {

                    index = (dirIndex + directions.Length + i) % directions.Length;
                    candidate = hub + directions[index];
                    if (Obstacles[candidate.x, candidate.y])
                    {
                        index = (dirIndex + directions.Length - i) % directions.Length;
                        candidate = hub + directions[index];

                        if (Obstacles[candidate.x, candidate.y])
                        {
                            continue;
                        }
                        else
                        {
                            
                            Concat(steps, CreatePath2(destination, candidate, index));
                            return steps;
                        }
                    }
                    else
                    {
                        Concat(steps, CreatePath2(destination, candidate, index));
                        return steps;
                    }
   
                }

                return steps;
            }

        } while (ray.Location() != destination && Iterations<80);
        
        //Other base case. destination reached
        return steps;
    }

    //public struct PathReturnValue
    //{
    //    public Stack<Vector2Int> path;
    //    public bool success;
    //}

    public bool CreatePath3(Vector2Int destination, Vector2Int location, ref Stack<Vector2Int> path, HashSet<int> visited = null, int dir = -1, int iterations = 0)
    {
        //Sort list of unvisited tiles within 1 step by distance to destination and test them in order of closeness to target.
        if (iterations == 32 || location == destination || Obstacles[destination.x, destination.y])
        {
            path.Push(location);
            return true;
        }

        if (location.x < 0 || location.y < 0 || location.x >= board.Dimensions.x || location.y >= board.Dimensions.y)
        {
           // Debug.Log("OUT OF BOUNDS");
            return false;
        }

        if (Obstacles[location.x, location.y])
        {
            //Debug.Log("HIT WALL");
            return false;
        }

        //if(path == null)
        //{
        //    path = new Stack<Vector2Int>();
        //}

        if (visited == null)
        {
            visited = new HashSet<int>();
        }

        //Debug.Log("dir = " + dir);

        float dist = Vector2Int.Distance(destination, location);
        Vector2Int candidate;
        int index = -1;
        //if (dir == -1)
        //{
            for (int i = 0; i < directions.Length; i++)
            {
                candidate = location + directions[i];
                if (Vector2Int.Distance(destination, candidate) < dist)
                {
                    index = i;
                    dist = Vector2Int.Distance(destination, candidate);
                }
            }
        //}
        //else
        //{
        //    index = dir;
        //}

        int limit;
        if (dir > -1) {
            limit = 3;//Mathf.Abs(index - dir);
            //Debug.Log("derived Limit: " + limit);
            path.Push(location);
            visited.Add(location.x * 8 + location.y);
        }
        else
        {
            limit = 4;//directions.Length / 2;
            //Debug.Log("initial Limit: " + limit);  
        }

        int back = (dir + 4 + directions.Length) % directions.Length;
        int tmpI;
        Vector2Int tmpV;

        //tmpI = (index + directions.Length) % directions.Length;
        tmpV = location + directions[index];

        if (!visited.Contains(tmpV.x * 8 + tmpV.y)/* && tmpI != back*/)
        {
            if (CreatePath3(destination, location + directions[index], ref path, visited, index, iterations + 1))
                return true;
        }

        
        //Debug.Log("Back = "+ back);
        for (int i = 1; i <=limit; i++)
        {
            tmpI = (index + i + directions.Length) % directions.Length;
            tmpV = location + directions[tmpI];
            if (!visited.Contains(tmpV.x*8+tmpV.y)/* && tmpI != back*/) {
                if (CreatePath3(destination, location + directions[tmpI], ref path, visited, tmpI, iterations + 1))
                    return true;
            }
            //else
            //{
            //    Debug.Log("CAN'T GO BACK");
            //}

            tmpI = (index - i + directions.Length) % directions.Length;
            tmpV = location + directions[tmpI];
            if (!visited.Contains(tmpV.x * 8 + tmpV.y)/* && tmpI != back*/)
            {
                if (CreatePath3(destination, location + directions[tmpI], ref path, visited, tmpI, iterations + 1))
                    return true;
            }
            //else
            //{
            //    Debug.Log("CAN'T GO BACK");
            //}
        }

        tmpI = (index + 4 + directions.Length) % directions.Length;
        tmpV = location + directions[tmpI];
        if (!visited.Contains(tmpV.x * 8 + tmpV.y)/* && tmpI != back*/)
        {
            if (CreatePath3(destination, location + directions[index], ref path, visited, index, iterations + 1))
                return true;
        }

        if (dir > -1)
        {
            path.Pop();
            visited.Remove(location.x * 8 + location.y);
        }

        return false;
    }

    
       
   
    

    void Concat(List<PathRay> a, List<PathRay> b)
    {
        for (int i = 0; i<b.Count; i++)
        {
            a.Add(b[i]);
        }
    }



    //Walks in a straight line from a point
    public class PathRay
    {
        Stack<Vector2Int> Path;

        Vector2Int Start;
        Vector2Int DominantVector;
        Vector2Int SecondaryVector;
        Vector2Int PendingVector;
        Vector2Int LastAddition;

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

                LastAddition = addition;
            }

            if (addition.magnitude > 0)
            {
                Path.Push(Start += addition);
                Debug.Log(Path.Peek());
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

        public int GetDirectionIndex()
        {
            for (int i = 0; i < directions.Length; i++)
            {
                if (directions[i] == LastAddition)
                {
                    return i;
                }
            }
            Debug.LogError("ERROR: direction " + LastAddition + " not found.");
            return -1;
        }


    }
}
