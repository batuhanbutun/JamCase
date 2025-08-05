using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingOnGridState : IPassengerState
{
    private readonly Passenger passenger;

    public WaitingOnGridState(Passenger p) { passenger = p; }

    public void Enter()
    {
        // Idle animasyonu, outline vs.
        
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        // Burada opsiyonel: fare hover, click feedback vb.
    }
}
