using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingToBenchState : IPassengerState
{
    private readonly Passenger passenger;
    private Coroutine moveCoroutine;
    private int myBenchIndex;
    public MovingToBenchState(Passenger p)
    {
        passenger = p;
    }
    
    public void Enter()
    {
        var targetBench = WaitingAreaManager.Instance.ReserveBenchForPassenger(passenger,out int benchIndex);
        BusManager.Instance.BusReadyToDepart += OnBusArrivingDuringMove;
        myBenchIndex = benchIndex;
        moveCoroutine = passenger.StartCoroutine(passenger.GetComponent<IMovable>()
            .Move(targetBench, OnArrivedAtBench));
    }

    public void Exit()
    {
        BusManager.Instance.BusReadyToDepart -= OnBusArrivingDuringMove;
        if (moveCoroutine != null)
        {
            passenger.StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    public void Tick()
    {
        
    }
    
    private void OnArrivedAtBench()
    {
        BusManager.Instance.BusReadyToDepart -= OnBusArrivingDuringMove;
        WaitingAreaManager.Instance.SetPassengerToBench();
        passenger.stateMachine.ChangeState(new WaitingOnBenchState(passenger,myBenchIndex));
    }
    
    private void OnBusArrivingDuringMove(Bus nextBus)
    {
        if (nextBus.ObjColor != passenger.ObjColor) return;
        
        if (moveCoroutine != null)
            passenger.StopCoroutine(moveCoroutine);
        
        WaitingAreaManager.Instance.ReleaseBench(myBenchIndex);
        passenger.stateMachine.ChangeState(new MovingToBusState(passenger));
    }
}
