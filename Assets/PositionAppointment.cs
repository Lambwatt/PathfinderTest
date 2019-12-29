using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAppointment
{
    public int Time;
    public int Position;
    public bool IsLast;

    PositionScheduler host;

    public PositionAppointment(PositionScheduler scheduler, int time, int position, bool isLast = false)
    {
        host = scheduler;
        Time = time;
        Position = position;
        IsLast = isLast;
    }

    public bool Update()
    {
        return --Time <= 0;
    }

    public void Cancel()
    {
        host.Cancel(Time, Position);
    }
}
