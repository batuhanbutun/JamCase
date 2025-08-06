using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatBusState : IPassengerState
{
    private readonly Passenger passenger;

    public SeatBusState(Passenger p)
    {
        passenger = p;
    }
    
    public void Enter()
    {
       BusManager.Instance.currentBus.SeatToBus(passenger);
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        
    }
}
