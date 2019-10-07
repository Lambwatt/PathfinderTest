using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector3 BottomLeftCorner;
    public Vector2 Intervals;
    public Vector2Int Dimensions;

    public GameObject positionMarker;

    GameObject[] Markers; 
    Vector3[,] Positions;

    // Start is called before the first frame update
    void Awake()
    {
        Markers = new GameObject[Dimensions.x * Dimensions.y];
        initialize();
    }

    private void initialize()
    {
        Positions = new Vector3[Dimensions.x, Dimensions.y];
        for (int i = 0; i < Dimensions.x; i++)
        {
            for (int j = 0; j < Dimensions.y; j++)
            {
                Positions[i, j] = BottomLeftCorner + new Vector3(i*Intervals.x, 1, j*Intervals.y);
                Markers[i*j+j]=Instantiate(positionMarker, Positions[i, j], Quaternion.identity);
            }
        }
    }

    public static int GetGCD(int x, int y)
    {
        while (y != 0)
        {
            int tmp = x % y;
            x = y;
            y = tmp;
        }
        return x;
    }

    public Vector4 CalculateDestination(Vector2Int start, Vector2Int end)
    {
        Vector2Int diff = end - start;

        int gcf = GetGCD(diff.x, diff.y);

        Vector4 res = new Vector4(diff.x / gcf, diff.y / gcf, diff.x % gcf, diff.y % gcf);

        return res;
    }

    public Vector2Int GetBoardCoordinates(Vector3 position)
    {
        Vector2Int res = new Vector2Int(
            Mathf.RoundToInt((position.x - BottomLeftCorner.x) / Intervals.x),
            Mathf.RoundToInt((position.z - BottomLeftCorner.z) / Intervals.y)
        );

        return res;
    }

    public Vector3 ConvertToWorldCoordinates(Vector2 boardPos)
    {
        Vector3 res = new Vector3(
               BottomLeftCorner.x + (boardPos.x * Intervals.x),
               1,
               BottomLeftCorner.z + (boardPos.y * Intervals.y)
        );

        return res;
    }

    public Vector3 GetMouseCoordinates()
    {
        //Debug.Log(Input.mousePosition);
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //position.x = Mathf.RoundToInt((position.x - BottomLeftCorner.x)/ Intervals.x);
        //position.x = BottomLeftCorner.x + (position.x * Intervals.x);

        //position.z = Mathf.RoundToInt((position.z - BottomLeftCorner.z) / Intervals.y);
        //position.z = BottomLeftCorner.z + (position.z * Intervals.y);

        //position.y = 1;

        return position;
    }

    void clearMarkers()
    {
        for (int m = 0; m < Markers.Length; m++)
        {
            if (Markers[m] != null)
            {
                Destroy(Markers[m]);
            }
            Markers[m] = null;
        }
    }

    public void Reset()
    {
        clearMarkers();
        initialize();
    }
}
