using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    public PathFollower pathFollower;
    public Board board;

    PositionAppointment pathAppointments; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //while moving, walk along path otherwise wait
    }

    public void Walk()
    {
        //Begin walking along path
    }
}
