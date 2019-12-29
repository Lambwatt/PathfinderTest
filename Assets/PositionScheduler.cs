using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//First assume units cannot change course
public class PositionScheduler
{
    Dictionary<int, PositionAppointment>[] Obstructions;

    public PositionScheduler(int max)
    {
        Obstructions = new Dictionary<int, PositionAppointment>[max];
    }

    public void Cancel(int timeIndex, int position)
    {
        Obstructions[timeIndex].Remove(position);
    }

    public void Update()
    {
        //convert keys from next time index to permanent keys on the 0
        if (Obstructions[1] != null) {
            Dictionary<int, PositionAppointment>.KeyCollection keys = Obstructions[1].Keys;

            foreach(int key in keys)
            {
                if (Obstructions[1][key].IsLast)
                {
                    Obstructions[0][key] = Obstructions[1][key];
                }
                else
                {
                    //destroy appointment?
                }
            }
        }

        //move every entery down 1 index
        for(int i = 2; i<=Obstructions.Length-1; i++)
        {
            Obstructions[i-1] = Obstructions[i];
        }
    }

    public bool CheckAppointment(int timeIndex, int position)
    {
        if (Obstructions[timeIndex] == null)
        {
            return false;
        }
        else if(Obstructions[timeIndex].ContainsKey(position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
