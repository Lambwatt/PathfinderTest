using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//First assume units cannot change course
public class PositionScheduler
{
    public Board Board;

    Dictionary<int, PositionAppointment>[] Obstructions;

    public PositionScheduler(int max)
    {
        Obstructions = new Dictionary<int, PositionAppointment>[max];
    }

    public void AddAppointment(Vector2Int location, int timeIndex, PositionAppointment appointment)
    {
        if(Obstructions[timeIndex] == null)
        {
            Obstructions[timeIndex] = new Dictionary<int, PositionAppointment>();
        }

        int key = ToKey(location);
        if (!Obstructions[timeIndex].ContainsKey(key))
        {
            Obstructions[timeIndex].Add(key, appointment);
        }
    }

    public void Cancel(int timeIndex, Vector2Int position)
    {
        Obstructions[timeIndex].Remove(ToKey(position));
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

    public bool CheckAppointment(int timeIndex, Vector2Int position)
    {
        if (Obstructions[timeIndex] == null)
        {
            return false;
        }
        else if(Obstructions[timeIndex].ContainsKey(ToKey(position)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int ToKey(Vector2Int position)
    {
        return position.x * Board.Dimensions.x + position.y;
    }
}
