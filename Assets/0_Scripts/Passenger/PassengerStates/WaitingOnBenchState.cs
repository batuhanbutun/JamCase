using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingOnBenchState : IPassengerState
{
    private readonly Passenger passenger;
    private Coroutine moveCoroutine;
    private int myBenchIndex;
    
    public WaitingOnBenchState(Passenger p,int benchIndex)
    {
        passenger = p;
        myBenchIndex = benchIndex;
    }

    public void Enter()
    {
        BusManager.Instance.BusReadyToDepart += OnBusComing;
    }

    public void Exit()
    {
        BusManager.Instance.BusReadyToDepart -= OnBusComing;
    }

    public void Tick()
    {
        
    }
    
    private void OnBusComing(Bus nextBus)
    {
        var available = BusManager.Instance.TryReceive(passenger);
        if (nextBus.ObjColor == passenger.ObjColor && available)
        {
            // Koltuğu boşalt
            WaitingAreaManager.Instance.ReleaseBench(myBenchIndex);
            // Bir sonraki state: bus’a yürü
            passenger.stateMachine.ChangeState(new MovingToBusState(passenger));
        }
    }
}
