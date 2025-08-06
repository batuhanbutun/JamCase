using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridEvents
{
    public static event Action<Passenger,Vector2Int> OnPassengerSent;
    public static event Action<Passenger> OnPassengerExitGrid;

    public static void PassengerSent(Passenger passenger,Vector2Int gridPos)
    {
        OnPassengerSent?.Invoke(passenger,gridPos);
    }
    
    public static void PassengerExitGrid(Passenger passenger)
    {
        OnPassengerExitGrid?.Invoke(passenger);
    }
}
