using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public PathAgent[] agents;
    bool moving;

    PositionScheduler PositionScheduler;

    // Start is called before the first frame update
    void Start()
    {
        PositionScheduler = new PositionScheduler(8 * 8);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && !moving)
        {
            foreach(PathAgent a in agents)
            {
                a.Walk();
            }

            //count down tick, at end of tick, update scheduler
        }
    }
}
