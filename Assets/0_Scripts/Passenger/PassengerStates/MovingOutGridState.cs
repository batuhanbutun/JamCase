using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingOutGridState : IPassengerState
{
    private readonly Passenger passenger;
    private readonly int startX, startZ;
    private readonly List<Vector2Int> path;
    private Coroutine co;
    public MovingOutGridState(Passenger p, int x, int z,List<Vector2Int> gridPath)
    {
        passenger = p;
        startX = x;
        startZ = z;
        path = gridPath;
    }


    public void Enter()
    {
        GridEvents.PassengerSent(passenger,new Vector2Int(startX, startZ));
        GridManager3D.Instance.gridPassengers[startX, startZ] = null;
        passenger.PassengerPathControl(false);
        Vector3[] worldPath = GridMovementPathfinder.GetPathWorldPoints(path);
        GridMovementPathfinder.EvaluatePassengerPaths();
        passenger.StartCoroutine(passenger.GetComponent<PassengerMovementController>().Move(worldPath,OnFinishedMoving));
    }

    public void Exit()
    {
        GridEvents.PassengerExitGrid(passenger);
    }

    public void Tick()
    {
        
    }
    
    private void OnFinishedMoving()
    {
        if (BusManager.Instance.TryReceive(passenger))
        {
            passenger.stateMachine.ChangeState(new MovingToBusState(passenger));
        }
        else if (WaitingAreaManager.Instance.TryReceive(passenger))
        {
            passenger.stateMachine.ChangeState(new MovingToBenchState(passenger));
        }
        else
        {
            GameManager.Instance.GameFail();
        }
    }
}
