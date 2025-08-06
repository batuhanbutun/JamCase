using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingToBusState : IPassengerState
{
    private readonly Passenger passenger;
    
    public MovingToBusState(Passenger p)
    {
        passenger = p;
    }

    public void Enter()
    {
        var targetBusPos = BusManager.Instance.currentBus.transform.position;
        passenger.StartCoroutine(passenger.GetComponent<IMovable>().Move(targetBusPos, () =>
        {
            passenger.stateMachine.ChangeState(new SeatBusState(passenger));
        }));
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        
    }
}
