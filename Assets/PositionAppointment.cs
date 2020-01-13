using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAppointment
{
    public int Time;
    public Vector2Int Position;
    public bool IsLast;

    PositionScheduler host;

    public PositionAppointment(PositionScheduler scheduler, int time, Vector2Int position, bool isLast = false)
    {
        host = scheduler;
        Time = time;
        Position = position;
        IsLast = isLast;
    }

    public bool Update()
    {
        return --Time <= 0; //is this useful?
    }

    public void Cancel()
    {
        host.Cancel(Time, Position);
    }
}
