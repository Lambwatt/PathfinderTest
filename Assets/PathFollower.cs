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

            List<Vector2Int> spots = new List<Vector2Int>();

            Stack<Vector2Int> tmp = new Stack<Vector2Int>();


            tmp = AStarProper(Location, Destination);
            
            while (tmp.Count > 0) {
                spots.Add(tmp.Pop());
            }

            //Clear path
            for (int i = PathMarks.Count - 1; i>=0; i--)
            {
                Destroy(PathMarks[i]);
            }
            PathMarks.Clear();
            
            //Create Path
            for(int i = 0; i<spots.Count; i++)
            {
                PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates(spots[i]) + new Vector3(0, 2, 0), Quaternion.identity));
                if (i > 0)
                {
                    PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)spots[i-1]+((Vector2)spots[i] - (Vector2)spots[i-1])*.25f) + new Vector3(0, 2, 0), Quaternion.identity));
                    PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)spots[i - 1] + ((Vector2)spots[i] - (Vector2)spots[i - 1]) * .5f) + new Vector3(0, 2, 0), Quaternion.identity));
                    PathMarks.Add(Instantiate(PathMarker, board.ConvertToWorldCoordinates((Vector2)spots[i - 1] + ((Vector2)spots[i] - (Vector2)spots[i - 1]) * .75f) + new Vector3(0, 2, 0), Quaternion.identity));
                }
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

    public Stack<Vector2Int> AStarProper(Vector2Int start, Vector2Int goal)
    {
        int width = 8;

        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        openSet.Add(start);

        Dictionary<int, float> gScore = new Dictionary<int, float>();
        Dictionary<int, float> fScore = new Dictionary<int, float>();
        Dictionary<int, Vector2Int> cameFrom = new Dictionary<int, Vector2Int>();

        SetScore(start, gScore, width, 0);
        SetScore(start, fScore, width, 0);

        while (openSet.Count > 0)
        {
            //current:= the node in openSet having the lowest fScore[] value
            Vector2Int current = GetNodeWithSmallestFScore(openSet, fScore, width);
            if (current == goal)
                return ReconstructPath(cameFrom, current, start, width);

            Vector2Int[] neighbors = new Vector2Int[8];
     
            for (int i = 0; i < directions.Length; i++)
            {
                neighbors[i] = current + directions[i];
            }

            openSet.Remove(current);
            foreach (Vector2Int n in neighbors)
            {
                if(n.x < 0 || n.y < 0 || n.x >= board.Dimensions.x || n.y >= board.Dimensions.y)
                    continue;

                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                float tentative_gScore = GetScore(current, gScore, width) + 1;
                if (tentative_gScore < GetScore(n, gScore, width))
                {
                    // This path to neighbor is better than any previous one. Record it!
                    int key = NodeToInt(n, width);
                    cameFrom[key] = current;
                    gScore[key] = tentative_gScore;
                    fScore[key] = gScore[key] + HeuristicFn(n, goal);
                    if (!openSet.Contains(n))
                    {
                        openSet.Add(n);
                    }
                }
            }
        }

        // Open set is empty but goal was never reached
        //return failure
        return null;
    }

    Stack<Vector2Int> ReconstructPath(Dictionary<int, Vector2Int> cameFrom, Vector2Int current, Vector2Int start, int width)
    {
        // total_path:= { current}
        //   while current in cameFrom.Keys:
        //    current:= cameFrom[current]
        //    total_path.prepend(current)
        //return total_path

        Stack<Vector2Int> path = new Stack<Vector2Int>();
        path.Push(current);

        while (cameFrom[NodeToInt(current, width)]!= start)
        {
            current = cameFrom[NodeToInt(current, width)];
            path.Push(current);
        }

        return path;
    }

    Vector2Int GetNodeWithSmallestFScore(HashSet<Vector2Int> set, Dictionary<int, float> fScore, int width)
    {
        Vector2Int res = new Vector2Int(-1,-1);
        foreach(Vector2Int node in set)
        {
            if(res == null)
            {
                res = node;
            }
            else
            {
                if (GetScore(node, fScore, width) < GetScore(res, fScore, width))
                    res = node;
            }
        }
        return res;
    }

    public float GetScore(Vector2Int node, Dictionary<int, float> gScore, int width)
    {
        int key = NodeToInt(node, width);
        if (gScore.ContainsKey(key))
            return gScore[key];
        else
            return int.MaxValue;
    }

    public float SetScore(Vector2Int node, Dictionary<int, float> gScore, int width, float value)
    {
        return gScore[NodeToInt(node, width)] = value;
    }

    public float HeuristicFn(Vector2Int node, Vector2Int destination)
    {
        Debug.Log("Testing [" + node.x + ", " + node.y + "]");
        return Obstacles[node.x, node.y] ? board.Dimensions.y*board.Dimensions.x : Vector2.Distance(node, destination);
    }

    public int NodeToInt(Vector2Int node, int width)
    {
        return node.x * width + node.y;
    }
}
